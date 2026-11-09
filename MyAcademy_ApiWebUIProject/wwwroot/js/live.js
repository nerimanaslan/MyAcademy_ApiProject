/* Serie A - canlı skor (SignalR)
   API'deki /hubs/match kanalını dinler:
   - "MatchChanged": sayfanın <main> içeriğini sunucudan yeniden alıp yerine koyar (sayfa yenilenmez)
   - "GoalScored"  : önce gol animasyonu (Şaolin modunda ateş topu), sonra içerik yenilenir
   Sayfa scriptleri "serie:content-updated" olayını dinleyerek kendilerini yeniden bağlar. */
(function () {
  'use strict';

  const hubUrl = document.body.dataset.hubUrl;
  if (!hubUrl || !window.signalR) return;

  /* ---------- Bağlantı durumu rozeti (menüde) ---------- */
  const badge = document.querySelector('[data-live-badge]');
  function setBadge(state) {
    if (!badge) return;
    badge.dataset.state = state;
    badge.title = { on: 'Canlı skor bağlantısı açık', off: 'Canlı skor bağlantısı yok', wait: 'Bağlanıyor...' }[state];
  }

  /* ---------- İçeriği sayfayı yenilemeden güncelle ---------- */
  let refreshTimer = null;
  function scheduleRefresh() {
    clearTimeout(refreshTimer);
    refreshTimer = setTimeout(refreshMain, 350);   // art arda gelen olayları tek istekte topla
  }

  async function refreshMain() {
    const current = document.querySelector('main');
    if (!current) return;
    try {
      const res = await fetch(location.href, { headers: { 'X-Requested-With': 'live' } });
      if (!res.ok) return;
      const doc = new DOMParser().parseFromString(await res.text(), 'text/html');
      const fresh = doc.querySelector('main');
      if (!fresh) return;
      current.innerHTML = fresh.innerHTML;
      document.title = doc.title;
      document.dispatchEvent(new CustomEvent('serie:content-updated'));
      flash(current);
    } catch (e) { /* ağ hatası: bir sonraki olayda tekrar denenir */ }
  }

  function flash(el) {
    el.classList.remove('live-flash');
    void el.offsetWidth;
    el.classList.add('live-flash');
  }

  /* ---------- Gol animasyonu ---------- */
  function playGoal(goal) {
    const team = goal.side === 'home' ? goal.home : goal.away;
    const score = `${goal.home.name} ${goal.homeScore} - ${goal.awayScore} ${goal.away.name}`;

    // Şaolin modu açıksa ateş topu, değilse Gol Sahnesi (scenes.js); ikisi de yoksa sade bant
    if (document.documentElement.dataset.shaolin === 'on' && window.Shaolin) {
      return window.Shaolin.fireballGoal(team, score);
    }
    if (window.Scenes && !window.Fx.reduce) {
      return window.Scenes.goal(goal);
    }

    const el = document.createElement('div');
    el.className = 'goal-banner';
    el.setAttribute('role', 'status');
    el.style.setProperty('--team-a', team.colorA);
    el.style.setProperty('--team-b', team.colorB);
    el.innerHTML = `<strong>GOL!</strong><span>${escapeHtml(team.name)}</span><small>${escapeHtml(score)}</small>`;
    document.body.appendChild(el);
    setTimeout(() => el.remove(), 3600);
    return new Promise((r) => setTimeout(r, 1200));
  }

  function escapeHtml(s) {
    return String(s).replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
  }
  window.SerieLive = { playGoal };   // konsoldan denemek için: SerieLive.playGoal({...})

  /* ---------- SignalR ---------- */
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
    .configureLogging(signalR.LogLevel.Warning)
    .build();

  connection.on('MatchChanged', scheduleRefresh);
  connection.on('GoalScored', async (goal) => {
    await playGoal(goal);
    scheduleRefresh();
  });

  connection.onreconnecting(() => setBadge('wait'));
  connection.onreconnected(() => { setBadge('on'); scheduleRefresh(); });
  connection.onclose(() => setBadge('off'));

  async function start() {
    setBadge('wait');
    try {
      await connection.start();
      setBadge('on');
    } catch (e) {
      setBadge('off');
      setTimeout(start, 10000);   // API kapalıysa 10 sn sonra tekrar dene
    }
  }
  start();
})();
