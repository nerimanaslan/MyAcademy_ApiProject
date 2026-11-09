/* Serie A - maç detayı: ağır çekim gol tekrarı + Chi Düellosu + kung-fu hareketleri
   - [data-replay] düğmesi ya da maç akışındaki gol işareti: sahada topun ateşli yolunu ağır çekimde çizer
   - [data-chi-duel]: ekrana girince iki Chi küresi çarpışır (effects.css)
   (kung-fu hareket listesi scenes.js içinde: window.KungFu)
   Topun yolu gol Id'sinden üretilir: aynı gol her tekrarda aynı yoldan gider. */
(function () {
  'use strict';

  const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  const NS = 'http://www.w3.org/2000/svg';
  const esc = (s) => String(s ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

  // Tohumlu rastgele sayı (mulberry32): aynı tohum -> aynı yol
  function rng(seed) {
    let a = seed >>> 0;
    return () => { a |= 0; a = a + 0x6D2B79F5 | 0; let t = Math.imul(a ^ a >>> 15, 1 | a); t = t + Math.imul(t ^ t >>> 7, 61 | t) ^ t; return ((t ^ t >>> 14) >>> 0) / 4294967296; };
  }
  const el = (name, attrs, parent) => { const n = document.createElementNS(NS, name); for (const k in attrs) n.setAttribute(k, attrs[k]); if (parent) parent.appendChild(n); return n; };
  const initials = (name) => String(name || '?').split(' ').filter(Boolean).map((w) => w[0]).slice(0, 2).join('').toUpperCase();

  /* ---------- Ağır çekim tekrar ---------- */
  function open(li) {
    const g = li.dataset, main = document.querySelector('[data-match-scene]');
    if (!main || !g.goalSeed) return;
    const m = main.dataset, home = g.goalSide === 'home';
    const teamA = home ? m.homeA : m.awayA, teamB = home ? m.homeB : m.awayB, teamCode = home ? m.homeCode : m.awayCode;
    const R = rng(Number(g.goalSeed) * 9973 + 17);

    // Ev sahibi sağdaki kaleye, deplasman soldakine hücum eder
    const dir = home ? 1 : -1, goalX = home ? 984 : 16;
    const sx = 500 + dir * (70 + R() * 230), sy = 110 + R() * 380;
    const ex = goalX, ey = 268 + R() * 64;
    const bend = (R() - .5) * 340;                                   // falsonun yönü ve şiddeti
    const cx = (sx + ex) / 2 - (ey - sy) * .15, cy = (sy + ey) / 2 + bend;
    const hasAssist = g.goalAssist && g.goalAssist.length;
    const ax = sx - dir * (90 + R() * 120), ay = Math.min(540, Math.max(60, sy + (R() - .5) * 260));

    const overlay = document.createElement('div');
    overlay.className = 'scene replay';
    overlay.setAttribute('role', 'dialog');
    overlay.setAttribute('aria-label', `${g.goalPlayer} golü, ağır çekim tekrar`);
    overlay.style.setProperty('--kf', g.goalColor);
    overlay.innerHTML = `
      <div class="replay__top">
        <span class="replay__rec">AĞIR ÇEKİM ×0.25</span>
        <span>${esc(m.homeName)} - ${esc(m.awayName)}</span>
        <span class="replay__minute">${esc(g.goalMinute)}'</span>
      </div>
      <div class="replay__pitch"><svg viewBox="0 0 1000 600" role="img" aria-label="Golün sahadaki yolu"></svg></div>
      <div class="replay__card">
        <span class="replay__move">${esc(g.goalIcon)} ${esc(g.goalMove)}</span>
        <span class="replay__who"><b>${esc(g.goalPlayer)}</b>${hasAssist ? ` · asist: ${esc(g.goalAssist)}` : ''}</span>
        <span class="replay__desc">${esc(g.goalDesc)}</span>
      </div>
      <div class="replay__actions">
        <button type="button" class="replay__btn" data-again>↻ Tekrar oynat</button>
        <button type="button" class="replay__btn" data-close>Kapat ✕</button>
      </div>`;
    document.body.appendChild(overlay);
    const svg = overlay.querySelector('svg'), card = overlay.querySelector('.replay__card');

    // Saha
    const defs = el('defs', {}, svg);
    const glow = el('filter', { id: 'rp-glow', x: '-50%', y: '-50%', width: '200%', height: '200%' }, defs);
    el('feGaussianBlur', { stdDeviation: '5', result: 'b' }, glow);
    const merge = el('feMerge', {}, glow); el('feMergeNode', { in: 'b' }, merge); el('feMergeNode', { in: 'SourceGraphic' }, merge);
    for (let i = 0; i < 10; i++) el('rect', { class: i % 2 ? 'rp-field-a' : 'rp-field-b', x: i * 100, y: 0, width: 100, height: 600 }, svg);
    el('rect', { class: 'rp-line', x: 30, y: 30, width: 940, height: 540 }, svg);
    el('line', { class: 'rp-line', x1: 500, y1: 30, x2: 500, y2: 570 }, svg);
    el('circle', { class: 'rp-line', cx: 500, cy: 300, r: 70 }, svg);
    [[30, 150, 150, 300], [820, 150, 150, 300], [30, 230, 55, 140], [915, 230, 55, 140]].forEach(([x, y, w, h]) => el('rect', { class: 'rp-line', x, y, width: w, height: h }, svg));
    const netL = el('rect', { class: 'rp-net', x: 8, y: 255, width: 22, height: 90 }, svg);
    const netR = el('rect', { class: 'rp-net', x: 970, y: 255, width: 22, height: 90 }, svg);
    const net = home ? netR : netL;
    net.style.setProperty('--net-kick', home ? '10px' : '-10px');

    // Asist pası + oyuncu
    if (hasAssist) {
      el('line', { class: 'rp-path', x1: ax, y1: ay, x2: sx, y2: sy }, svg);
      el('circle', { class: 'rp-player', cx: ax, cy: ay, r: 18, fill: teamA, opacity: .75 }, svg);
      el('text', { class: 'rp-player-label', x: ax, y: ay + 6 }, svg).textContent = initials(g.goalAssist);
    }
    const d = `M${sx},${sy} Q${cx},${cy} ${ex},${ey}`;
    el('path', { class: 'rp-path', d }, svg);
    const trailOuter = el('path', { class: 'rp-trail', d, stroke: g.goalColor, 'stroke-width': 16, opacity: .85 }, svg);
    const trailInner = el('path', { class: 'rp-trail', d, stroke: '#fff', 'stroke-width': 5 }, svg);
    const shooter = el('circle', { class: 'rp-player', cx: sx, cy: sy, r: 22, fill: teamA }, svg);
    shooter.style.stroke = teamB;
    el('text', { class: 'rp-player-label', x: sx, y: sy + 6 }, svg).textContent = initials(g.goalPlayer);
    const ghosts = [4, 3, 2, 1].map((k) => el('circle', { r: 9 - k, fill: g.goalColor, opacity: .12 * (5 - k) }, svg));
    const ball = el('circle', { r: 10, fill: '#fff', stroke: '#111', 'stroke-width': 2 }, svg);
    const len = trailOuter.getTotalLength();
    [trailOuter, trailInner].forEach((p) => { p.style.strokeDasharray = len; });
    const label = el('text', { class: 'rp-player-label', x: 500, y: 20, opacity: .7 }, svg);
    label.textContent = teamCode + (home ? ' ► hücum yönü' : ' ◄ hücum yönü');

    let raf = 0;
    function draw(t) {
      [trailOuter, trailInner].forEach((p) => { p.style.strokeDashoffset = len * (1 - t); });
      const pt = trailOuter.getPointAtLength(len * t);
      ball.setAttribute('cx', pt.x); ball.setAttribute('cy', pt.y);
      ghosts.forEach((gh, i) => { const q = trailOuter.getPointAtLength(len * Math.max(0, t - (i + 1) * .035)); gh.setAttribute('cx', q.x); gh.setAttribute('cy', q.y); });
    }
    function finish() {
      draw(1);
      net.classList.remove('is-hit'); void net.getBBox(); net.classList.add('is-hit');
      card.classList.add('is-in');
      if (window.Fx && !reduce) {
        const r = net.getBoundingClientRect();
        Fx.confetti([teamA, teamB, g.goalColor], { count: 90, frames: 140, origins: [{ x: (r.left + r.width / 2) / innerWidth, y: (r.top + r.height / 2) / innerHeight, dir: 0 }] });
      }
    }
    function play() {
      cancelAnimationFrame(raf); card.classList.remove('is-in');
      if (reduce) { finish(); return; }
      const start = performance.now(), dur = 2600;
      shooter.style.transformBox = 'fill-box'; shooter.style.transformOrigin = 'center';   // vuruş anı: oyuncu irkilir
      shooter.animate([{ transform: 'scale(1)' }, { transform: 'scale(1.35)' }, { transform: 'scale(1)' }], { duration: 400 });
      (function step(now) {
        const x = Math.min(1, (now - start) / dur);
        draw(x < .5 ? 2 * x * x : 1 - Math.pow(-2 * x + 2, 2) / 2);   // yavaş başla, yavaş bitir
        if (x < 1) raf = requestAnimationFrame(step); else finish();
      })(start);
    }

    const close = () => { cancelAnimationFrame(raf); document.removeEventListener('keydown', onKey); overlay.remove(); };
    const onKey = (e) => { if (e.key === 'Escape') close(); };
    document.addEventListener('keydown', onKey);
    overlay.querySelector('[data-close]').addEventListener('click', close);
    overlay.querySelector('[data-again]').addEventListener('click', play);
    overlay.addEventListener('click', (e) => { if (e.target === overlay) close(); });
    overlay.querySelector('[data-close]').focus();
    play();
  }

  // Tekrar düğmeleri + maç akışındaki gol işaretleri
  document.addEventListener('click', (e) => {
    const btn = e.target.closest('[data-replay]');
    const marker = !btn && e.target.closest('.flow-marker--goal');
    const id = btn ? btn.dataset.replay : marker ? marker.dataset.flowRef : null;
    if (!id) return;
    const li = document.getElementById(id);
    if (li && li.dataset.goalSeed) open(li);
  });

  /* ---------- Chi Düellosu ---------- */
  function initChi() {
    const duel = document.querySelector('[data-chi-duel]');
    if (!duel || duel.classList.contains('is-in')) return;
    if (reduce) { duel.classList.add('is-in'); return; }
    const io = new IntersectionObserver((entries) => {
      if (!entries[0].isIntersecting) return;
      io.disconnect();
      duel.classList.add('is-in');
    }, { threshold: .5 });
    io.observe(duel);
  }
  initChi();
  document.addEventListener('serie:content-updated', () => {
    const duel = document.querySelector('[data-chi-duel]');
    if (duel) duel.classList.add('is-in');   // canlı güncellemede animasyonsuz
  });

  window.Replay = { open };
})();
