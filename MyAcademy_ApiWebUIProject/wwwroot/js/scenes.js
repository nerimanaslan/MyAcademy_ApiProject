/* Serie A - sahne şovları
   Scenes.matchIntro() : maç detayına girişte iki takım rengi çarpışır, armalar ortada buluşur, skor yuvarlanır
   Scenes.goal(goal)   : canlı skorda gol -> takım renginde şeritler, dev "GOL!", konfeti
   Scenes.brush(on)    : Şaolin Modu açılırken mürekkep fırça darbesi
   Kürsü               : puan durumunda ilk 3 takım görününce yükselir + liderde konfeti
   Her sahne "Geç" düğmesi, tıklama veya Esc ile atlanabilir. Hareket azaltma tercihinde gösterilmez. */
(function () {
  'use strict';

  const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  // Kung-fu gol hareketleri: ViewModels/KungFuMoves.cs ile aynı liste ve sıra
  const MOVES = [
    ['Ejder Vuruşu', '🐉', '#FF6A00'], ['Kartal Pençesi', '🦅', '#E8A317'], ['Rüzgar Tekmesi', '🌪️', '#5BC0EB'],
    ['Kaplan Sıçrayışı', '🐯', '#F2994A'], ['Kobra Dokunuşu', '🐍', '#1FC98A'], ['Turna Kanadı', '🕊️', '#C9D4EA'],
    ['Demir Kafa', '🛡️', '#A9BCDF'], ['Gölge Ayak', '🌑', '#9D7BFF']
  ].map(([name, icon, color]) => ({ name, icon, color }));
  // Canlı gol (admin skoru artırdı, henüz gol kaydı yok): maç ve skordan hareket seç
  window.KungFu = { moves: MOVES, forLive: (g) => MOVES[((g.matchId || 0) * 7 + g.homeScore * 3 + g.awayScore * 5) % MOVES.length] };

  const esc = (s) => String(s ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

  // Ortak sahne kabuğu: oluştur, atla, kapat
  function stage(className, html, duration, onDone) {
    const el = document.createElement('div');
    el.className = 'scene ' + className;
    el.setAttribute('role', 'dialog');
    el.setAttribute('aria-label', 'Sahne animasyonu');
    el.innerHTML = html + '<button type="button" class="scene__skip">Geç ▸</button>';
    document.body.appendChild(el);

    let closed = false;
    const close = () => {
      if (closed) return; closed = true;
      el.classList.add('is-leaving');
      document.removeEventListener('keydown', onKey);
      setTimeout(() => { el.remove(); if (onDone) onDone(); }, 520);
    };
    const onKey = (e) => { if (e.key === 'Escape') close(); };
    el.addEventListener('click', close);
    document.addEventListener('keydown', onKey);
    const timer = setTimeout(close, duration);
    return { el, close: () => { clearTimeout(timer); close(); } };
  }

  /* ---------- Maç Sahnesi ---------- */
  function matchIntro() {
    const main = document.querySelector('[data-match-scene]');
    if (!main || reduce) return;
    const d = main.dataset;
    const key = 'serieA.scene.match.' + d.matchId;
    try { if (sessionStorage.getItem(key)) return; sessionStorage.setItem(key, '1'); } catch (e) { /* yoksa her seferinde oynar */ }

    const hasScore = d.homeScore !== '' && d.awayScore !== '';
    const center = hasScore
      ? `<div class="match-scene__score"><span data-roll="${esc(d.homeScore)}">0</span><span>-</span><span data-roll="${esc(d.awayScore)}">0</span></div>`
      : '<div class="match-scene__vs">VS</div>';

    const s = stage('match-scene', `
      <div class="match-scene__half match-scene__half--home"></div>
      <div class="match-scene__half match-scene__half--away"></div>
      <div class="match-scene__stripe"></div>
      <div class="match-scene__ring"></div>
      <div class="match-scene__team match-scene__team--home">
        <span class="match-scene__crest" style="--crest-a:${esc(d.homeA)};--crest-b:${esc(d.homeB)}">${esc(d.homeCode)}</span>
        <span class="match-scene__name">${esc(d.homeName)}</span>
      </div>
      <div class="match-scene__team match-scene__team--away">
        <span class="match-scene__crest" style="--crest-a:${esc(d.awayA)};--crest-b:${esc(d.awayB)}">${esc(d.awayCode)}</span>
        <span class="match-scene__name">${esc(d.awayName)}</span>
      </div>
      <div class="match-scene__center">
        ${d.derby ? `<div class="match-scene__derby">🔥 ${esc(d.derby)}</div>` : ''}
        ${center}
        <div class="match-scene__meta">${esc(d.week)}. Hafta · ${esc(d.stadium)} · ${esc(d.status)}</div>
      </div>`, hasScore ? 3800 : 3000);

    s.el.style.setProperty('--home-a', d.homeA);
    s.el.style.setProperty('--away-a', d.awayA);

    // Skor: çarpışmadan sonra sayılar yuvarlanarak gelir
    if (hasScore) {
      setTimeout(() => {
        s.el.querySelectorAll('[data-roll]').forEach((span) => {
          const target = Number(span.dataset.roll); let v = 0;
          if (!target) return;
          const iv = setInterval(() => { span.textContent = ++v; if (v >= target) clearInterval(iv); }, 260);
        });
      }, 1000);
    }
  }

  /* ---------- Gol Sahnesi ---------- */
  function goal(g) {
    const team = g.side === 'home' ? g.home : g.away;
    if (reduce) return Promise.resolve();
    const word = 'GOL!'.split('').map((c, i) => `<span style="--i:${i}">${c}</span>`).join('');
    const homeNew = g.side === 'home' ? ' is-new' : '', awayNew = g.side === 'away' ? ' is-new' : '';
    const move = KungFu.forLive(g);
    const moveText = `${move.icon} ${move.name}`;

    const s = stage('goal-scene', `
      <div class="goal-scene__wipe">${'<span></span>'.repeat(9)}</div>
      <div class="goal-scene__content">
        <span class="goal-scene__crest">${esc(team.code)}</span>
        <div class="goal-scene__word">${word}</div>
        <div class="goal-scene__move">${esc(moveText)}</div>
        <div class="goal-scene__team">${esc(team.name)}</div>
        <div class="goal-scene__score">${esc(g.home.name)} <b class="${homeNew.trim()}">${g.homeScore}</b> - <b class="${awayNew.trim()}">${g.awayScore}</b> ${esc(g.away.name)}</div>
      </div>`, 3600);

    s.el.style.setProperty('--team-a', team.colorA);
    s.el.style.setProperty('--team-b', team.colorB);
    // şeritler sırayla düşsün
    s.el.querySelectorAll('.goal-scene__wipe span').forEach((sp, i) => { sp.style.animationDelay = (i * 0.035) + 's'; });
    setTimeout(() => window.Fx && Fx.confetti([team.colorA, team.colorB, '#FFD25A'], { count: 200 }), 900);

    return new Promise((r) => setTimeout(r, 1800));   // içerik yenilemesi gol yazısından sonra
  }

  /* ---------- Şaolin fırça geçişi ---------- */
  function brush(onMiddle) {
    if (reduce) { onMiddle(); return; }
    const s = stage('brush-scene', `
      <svg viewBox="0 0 1600 900" preserveAspectRatio="none" aria-hidden="true">
        <defs>
          <filter id="brush-rough"><feTurbulence type="fractalNoise" baseFrequency=".035" numOctaves="2" result="n"/><feDisplacementMap in="SourceGraphic" in2="n" scale="38"/></filter>
        </defs>
        <path class="brush-ink" d="M-150 620 C 250 380, 520 760, 820 480 S 1350 260, 1760 420"/>
        <path class="brush-red" d="M-120 700 C 300 520, 640 820, 960 600 S 1420 430, 1760 560"/>
      </svg>
      <div class="brush-scene__title">ŞAOLİN MODU</div>`, 1900);
    setTimeout(onMiddle, 520);   // tema, fırça ekranı kapladığında değişir
    return s;
  }

  /* ---------- Kürsü ---------- */
  function podium() {
    const el = document.querySelector('[data-podium]');
    if (!el) return;
    if (reduce) { el.classList.add('is-in'); return; }
    const io = new IntersectionObserver((entries) => {
      if (!entries[0].isIntersecting) return;
      io.disconnect();
      el.classList.add('is-in');
      const leader = el.querySelector('.podium__place--1');
      let first = true;
      try { first = !sessionStorage.getItem('serieA.scene.podium'); sessionStorage.setItem('serieA.scene.podium', '1'); } catch (e) { }
      if (leader && first) {
        // konfeti liderin kürsüsünden fışkırır
        setTimeout(() => {
          const r = leader.getBoundingClientRect();
          Fx.confetti([leader.dataset.colorA, leader.dataset.colorB, '#FFD25A'], {
            count: 140, frames: 170,
            origins: [{ x: (r.left + r.width / 2) / innerWidth, y: Math.min(1, (r.bottom - 40) / innerHeight), dir: 0 }]
          });
        }, 1500);
      }
    }, { threshold: .35 });
    io.observe(el);
  }

  window.Scenes = { matchIntro, goal, brush };

  matchIntro();
  podium();
  document.addEventListener('serie:content-updated', () => {
    const el = document.querySelector('[data-podium]');
    if (el) el.classList.add('is-in');   // canlı güncellemede tekrar oynatma
  });
})();
