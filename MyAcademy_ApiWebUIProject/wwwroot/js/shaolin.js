/* Serie A - Şaolin Modu
   - Menüdeki "🥋 Şaolin Modu" düğmesi temayı açar/kapatır (tercih localStorage'da saklanır)
   - data-shaolin-text="..." olan öğelerin yazısı mod açıkken bu metne döner
   - Ustanın sözü: mod açıkken menünün altında her sayfada farklı bir söz
   - Shaolin.fireballGoal(takım, skor): canlı skorda gol olunca ateş topu animasyonu (live.js çağırır) */
(function () {
  'use strict';

  const KEY = 'serieA.shaolin';
  const root = document.documentElement;
  const isOn = () => root.dataset.shaolin === 'on';

  /* ---------- Yazı değişimleri ---------- */
  function applyTexts() {
    document.querySelectorAll('[data-shaolin-text]').forEach((el) => {
      if (el.dataset.originalText === undefined) el.dataset.originalText = el.textContent;
      el.textContent = isOn() ? el.dataset.shaolinText : el.dataset.originalText;
    });
  }

  /* ---------- Ustanın sözü ---------- */
  const QUOTES = [
    'Top ayağına gelmeden önce zihnine gelir.',
    'Bin şut çeken değil, tek şutu doğru çeken kazanır.',
    'Savunma bir duvar değil, bir nefestir.',
    'Takım bir yumruktur; beş parmak ayrı ayrı vuramaz.',
    'Puan tablosu sabırla yükselir, öfkeyle düşer.',
    'Kaleci kapıyı değil, korkuyu kapatır.',
    'Ateşli şut ayaktan değil, kalpten çıkar.',
    'Ertelenen maç, ustanın sabrını sınar.'
  ];
  function renderQuote() {
    let el = document.querySelector('[data-master-quote]');
    if (!el) {
      const nav = document.querySelector('.public-nav');
      if (!nav) return;
      el = document.createElement('p');
      el.className = 'master-quote shaolin-only';
      el.dataset.masterQuote = '';
      nav.insertAdjacentElement('afterend', el);
    }
    const q = QUOTES[Math.floor(Math.random() * QUOTES.length)];
    el.innerHTML = `<b>USTA DER Kİ:</b>${q}`;
  }

  /* ---------- Aç / kapat ---------- */
  const toggle = document.querySelector('[data-shaolin-toggle]');
  function setMode(on) {
    if (on) root.dataset.shaolin = 'on'; else delete root.dataset.shaolin;
    try { localStorage.setItem(KEY, on ? 'on' : 'off'); } catch (e) { /* gizli sekme vb. */ }
    if (toggle) toggle.setAttribute('aria-pressed', String(on));
    applyTexts();
  }
  if (toggle) {
    toggle.setAttribute('aria-pressed', String(isOn()));
    toggle.addEventListener('click', () => {
      if (isOn()) { setMode(false); return; }
      // açılırken: mürekkep fırçası ekranı kaplar, tema arkada değişir (scenes.js)
      if (window.Scenes) window.Scenes.brush(() => setMode(true));
      else setMode(true);
    });
  }

  /* ---------- Ateş topu gol animasyonu ---------- */
  function fireballGoal(team, scoreText, title) {
    const stage = document.createElement('div');
    stage.className = 'fireball-stage';
    stage.setAttribute('role', 'status');
    stage.style.setProperty('--team-a', team.colorA || '#C81E1E');
    stage.style.setProperty('--team-b', team.colorB || '#FFC94D');

    let sparks = '';
    for (let i = 0; i < 16; i++) {
      const angle = (Math.PI * 2 * i) / 16, dist = 120 + Math.random() * 220;
      sparks += `<span class="fireball-spark" style="--dx:${Math.cos(angle) * dist}px;--dy:${Math.sin(angle) * dist}px"></span>`;
    }
    stage.innerHTML = `
      <span class="fireball"></span>
      <span class="fireball-burst"></span>
      ${sparks}
      <div class="fireball-text">
        <em>ATEŞLİ ŞUT</em>
        <strong>${esc(title || 'GOOOL!')}</strong>
        <span>${esc(team.name)}</span>
        <small>${esc(scoreText)}</small>
      </div>`;
    document.body.appendChild(stage);
    document.body.classList.remove('is-shaking');
    void document.body.offsetWidth;
    document.body.classList.add('is-shaking');

    return new Promise((resolve) => {
      setTimeout(resolve, 1500);                    // içerik yenilemesi patlamadan sonra başlasın
      setTimeout(() => { stage.remove(); document.body.classList.remove('is-shaking'); }, 4000);
    });
  }

  function esc(s) {
    return String(s).replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
  }

  window.Shaolin = { fireballGoal, setMode };

  applyTexts();
  renderQuote();
  document.addEventListener('serie:content-updated', applyTexts);
})();
