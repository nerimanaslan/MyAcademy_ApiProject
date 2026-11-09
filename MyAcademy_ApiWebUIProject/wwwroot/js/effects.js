/* Serie A - genel görsel efektler (tüm herkese açık sayfalar)
   - Kaydırınca beliren kartlar (sırayla, kademeli)
   - Sayıların 0'dan yukarı sayması (istatistikler, puanlar)
   - Kartlarda fareye göre 3B eğim
   - Fx.confetti(renkler): ekran üstü konfeti (gol ve kürsü sahneleri kullanır)
   Hareket azaltma tercihi açık olan ziyaretçide hiçbiri çalışmaz. */
(function () {
  'use strict';

  const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  const canHover = window.matchMedia('(hover: hover)').matches;

  /* ---------- Konfeti (canvas) ---------- */
  function confetti(colors, opts = {}) {
    if (reduce) return;
    const canvas = document.createElement('canvas');
    canvas.className = 'fx-confetti';
    document.body.appendChild(canvas);
    const ctx = canvas.getContext('2d');
    const dpr = Math.min(window.devicePixelRatio || 1, 2);
    const W = canvas.width = innerWidth * dpr, H = canvas.height = innerHeight * dpr;
    const palette = (colors && colors.length ? colors : ['#FFD25A', '#fff', '#2E7BFF']).concat('#ffffff');
    const count = opts.count || 160;
    const origins = opts.origins || [{ x: 0, y: 1, dir: 1 }, { x: 1, y: 1, dir: -1 }];   // ekranın alt köşeleri

    const parts = Array.from({ length: count }, (_, i) => {
      const o = origins[i % origins.length];
      const angle = (-70 + Math.random() * 40) * Math.PI / 180;   // köşelerden: yukarı ve içe doğru
      const speed = (14 + Math.random() * 14) * dpr;
      const fountain = o.dir === 0;                                // ortadan: yukarı fışkırır, iki yana açılır
      return {
        x: o.x * W, y: o.y * H,
        vx: fountain ? (Math.random() - .5) * speed * 1.1 : Math.cos(angle) * speed * o.dir,
        vy: fountain ? -speed * (0.8 + Math.random() * 0.5) : Math.sin(angle) * speed,
        w: (6 + Math.random() * 8) * dpr, h: (8 + Math.random() * 10) * dpr,
        rot: Math.random() * Math.PI, vr: (Math.random() - .5) * .3,
        color: palette[i % palette.length], life: 0
      };
    });

    let frame = 0;
    (function tick() {
      ctx.clearRect(0, 0, W, H);
      parts.forEach((p) => {
        p.vy += .45 * dpr; p.vx *= .99; p.vy *= .99;
        p.x += p.vx; p.y += p.vy; p.rot += p.vr;
        ctx.save(); ctx.translate(p.x, p.y); ctx.rotate(p.rot);
        ctx.fillStyle = p.color;
        ctx.fillRect(-p.w / 2, -p.h / 2, p.w, p.h * Math.abs(Math.cos(p.rot * 2)));   // dönerken incelir
        ctx.restore();
      });
      if (++frame < (opts.frames || 210)) requestAnimationFrame(tick);
      else canvas.remove();
    })();
  }
  window.Fx = { confetti, reduce };
  if (reduce) return;

  /* ---------- Kaydırınca beliren öğeler ---------- */
  const REVEAL = '.panel, .stat-card, .table-panel, .side-card, .period-card, .chi-meter, .fixture-card, .podium, .week-info, .week-nav, .club-stats > div';
  const observer = new IntersectionObserver((entries) => {
    entries.forEach((e) => {
      if (!e.isIntersecting) return;
      e.target.classList.add('is-in');
      observer.unobserve(e.target);
    });
  }, { threshold: .12, rootMargin: '0px 0px -40px 0px' });

  // her sayı ekrana girdiği an saymaya başlar
  const countObserver = new IntersectionObserver((entries) => {
    entries.forEach((e) => {
      if (!e.isIntersecting) return;
      countOne(e.target);
      countObserver.unobserve(e.target);
    });
  }, { threshold: .5 });

  function setupReveal(root, instant) {
    const groups = new Map();
    root.querySelectorAll(REVEAL).forEach((el) => {
      if (el.classList.contains('fx-reveal')) return;
      el.classList.add('fx-reveal');
      // aynı ebeveyndeki kardeşler sırayla gelsin
      const parent = el.parentElement;
      const i = groups.get(parent) || 0; groups.set(parent, i + 1);
      el.style.setProperty('--fx-delay', Math.min(i, 8) * 0.06 + 's');
      if (instant) el.classList.add('is-in'); else observer.observe(el);
    });
  }

  /* ---------- Sayıların sayarak gelmesi ---------- */
  const COUNT_SEL = '.stat-card__value, .hero-meta dd, .summary-list dd, .club-stats dd, .standings-table .col-pts, .period-card__score';
  function markCounters(root) {
    root.querySelectorAll(COUNT_SEL).forEach((el) => {
      const node = [...el.childNodes].find((n) => n.nodeType === 3 && n.textContent.trim());
      if (!node || !/^[+]?\d{1,4}\.?$/.test(node.textContent.trim())) return;   // "2026/27", "%54" gibi değerleri atla
      el.dataset.count = node.textContent.trim();
    });
  }
  function countOne(el) {
    if (el.dataset.counted) return;
    el.dataset.counted = '1';
    const node = [...el.childNodes].find((n) => n.nodeType === 3 && n.textContent.trim());
    if (!node) return;
    const raw = el.dataset.count, plus = raw.startsWith('+') ? '+' : '', dot = raw.endsWith('.') ? '.' : '';
    const target = parseInt(raw.replace(/[^\d]/g, ''), 10);
    const start = performance.now(), dur = 900 + Math.min(target, 100) * 4;
    (function step(now) {
      const t = Math.min(1, (now - start) / dur), eased = 1 - Math.pow(1 - t, 3);
      node.textContent = plus + Math.round(target * eased) + dot;
      if (t < 1) requestAnimationFrame(step);
    })(start);
  }
  function setupCounters(root, instant) {
    markCounters(root);
    root.querySelectorAll('[data-count]').forEach((el) => {
      if (instant) { el.dataset.counted = '1'; return; }   // canlı güncellemede tekrar sayma
      const node = [...el.childNodes].find((n) => n.nodeType === 3 && n.textContent.trim());
      const raw = el.dataset.count;
      node.textContent = (raw.startsWith('+') ? '+' : '') + '0' + (raw.endsWith('.') ? '.' : '');   // görünene kadar 0
      countObserver.observe(el);
    });
  }

  /* ---------- 3B eğim ---------- */
  const TILT = '.fixture-card, .stat-card, .period-card, .podium__place';
  function setupTilt(root) {
    if (!canHover) return;
    root.querySelectorAll(TILT).forEach((el) => {
      if (el.dataset.tilt) return;
      el.dataset.tilt = '1';
      el.classList.add('fx-tilt');
      el.addEventListener('mousemove', (e) => {
        const r = el.getBoundingClientRect();
        const px = (e.clientX - r.left) / r.width - .5, py = (e.clientY - r.top) / r.height - .5;
        el.classList.add('is-tilting');
        el.style.transform = `perspective(900px) rotateY(${px * 7}deg) rotateX(${-py * 7}deg) translateZ(4px)`;
      });
      el.addEventListener('mouseleave', () => { el.classList.remove('is-tilting'); el.style.transform = ''; });
    });
  }

  function init(instant) {
    const main = document.querySelector('main');
    if (!main) return;
    setupCounters(main, instant);
    setupReveal(main, instant);
    setupTilt(main);
  }

  init(false);
  // canlı güncellemede yeni içerik animasyonsuz görünsün (göz yormasın)
  document.addEventListener('serie:content-updated', () => init(true));
})();
