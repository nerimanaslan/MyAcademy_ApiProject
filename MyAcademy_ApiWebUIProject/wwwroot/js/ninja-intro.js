/* =========================================================
   Ninja açılış animasyonu
   - Sarı kıyafetli ninja topa vurur
   - Top alevler içinde ekrana doğru gelir, çarpınca tüm ekran ateşle kaplanır
   - Arkada kırmızı bir ejderha ninjanın etrafında S çizerek döner
   Oturum başına yalnızca bir kez (ilk açılışta) oynar.
   ========================================================= */
(function () {
  'use strict';

  var KEY = 'njIntroSeen_v2';
  try { if (sessionStorage.getItem(KEY)) return; } catch (e) { /* storage yoksa yine oynat */ }
  function markSeen() { try { sessionStorage.setItem(KEY, '1'); } catch (e) { } }

  if (window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches) { markSeen(); return; }
  if (!document.body || !window.requestAnimationFrame) return;

  /* ---------- Yardımcılar ---------- */
  var PI = Math.PI, TAU = PI * 2;
  function rnd(a, b) { return a + Math.random() * (b - a); }
  function clamp01(x) { return x < 0 ? 0 : x > 1 ? 1 : x; }
  function easeOut(p) { return 1 - Math.pow(1 - p, 3); }
  function easeInOut(p) { return p < .5 ? 4 * p * p * p : 1 - Math.pow(-2 * p + 2, 3) / 2; }
  function f(n) { return n.toFixed(1); }

  /* ---------- Top (futbol topu) SVG ---------- */
  function ballMarkup(id, hot) {
    var s = '<defs>' +
      '<clipPath id="njClip' + id + '"><circle r="47"/></clipPath>' +
      '<radialGradient id="njShade' + id + '" cx="35%" cy="30%" r="78%">' +
      '<stop offset="0" stop-color="#ffffff"/><stop offset=".68" stop-color="#e8ecf2"/><stop offset="1" stop-color="#8f99a8"/>' +
      '</radialGradient>' +
      '<radialGradient id="njHot' + id + '" cx="50%" cy="50%" r="50%">' +
      '<stop offset=".35" stop-color="rgba(255,170,40,0)"/><stop offset=".8" stop-color="rgba(255,110,0,.55)"/><stop offset="1" stop-color="rgba(255,60,0,.9)"/>' +
      '</radialGradient></defs>' +
      '<circle r="47" fill="url(#njShade' + id + ')"/>' +
      '<g clip-path="url(#njClip' + id + ')" fill="#151515" stroke="#151515" stroke-width="3" stroke-linejoin="round" stroke-linecap="round">';
    var inner = [], i, k, a, b;
    for (i = 0; i < 5; i++) {
      a = (-90 + i * 72) * PI / 180;
      inner.push([Math.cos(a) * 17, Math.sin(a) * 17]);
    }
    s += '<polygon points="' + inner.map(function (p) { return f(p[0]) + ',' + f(p[1]); }).join(' ') + '"/>';
    for (i = 0; i < 5; i++) {
      a = (-90 + i * 72) * PI / 180;
      var cx = Math.cos(a) * 48, cy = Math.sin(a) * 48, pts = [];
      for (k = 0; k < 5; k++) {
        b = a + PI + k * 72 * PI / 180;
        pts.push(f(cx + Math.cos(b) * 15) + ',' + f(cy + Math.sin(b) * 15));
      }
      s += '<polygon points="' + pts.join(' ') + '"/>';
      s += '<line x1="' + f(inner[i][0]) + '" y1="' + f(inner[i][1]) + '" x2="' + f(Math.cos(a) * 33) + '" y2="' + f(Math.sin(a) * 33) + '"/>';
    }
    s += '</g>';
    if (hot) s += '<circle r="47" fill="url(#njHot' + id + ')"/>';
    s += '<circle r="47" fill="none" stroke="' + (hot ? '#5a1600' : '#111') + '" stroke-width="2.5"/>';
    return s;
  }

  /* ---------- Sarı kıyafetli ninja ---------- */
  var GI = 'url(#njGi)';
  var ninjaSvg =
    '<svg class="nj-ninja" viewBox="0 0 220 280" aria-hidden="true">' +
    '<defs>' +
    '<linearGradient id="njGi" x1="0" y1="0" x2="1" y2="1"><stop offset="0" stop-color="#FFE45C"/><stop offset="1" stop-color="#F2A900"/></linearGradient>' +
    '<linearGradient id="njGiDark" x1="0" y1="0" x2="1" y2="1"><stop offset="0" stop-color="#E9B200"/><stop offset="1" stop-color="#C98600"/></linearGradient>' +
    '</defs>' +
    '<ellipse cx="114" cy="268" rx="64" ry="8" fill="rgba(0,0,0,.5)"/>' +
    '<g id="njBody">' +
      '<g id="njTails">' +
        '<path d="M86 54 C 66 46, 50 56, 30 44" fill="none" stroke="#141414" stroke-width="7" stroke-linecap="round"/>' +
        '<path d="M86 58 C 68 64, 52 70, 36 64" fill="none" stroke="#141414" stroke-width="6" stroke-linecap="round"/>' +
      '</g>' +
      '<path d="M94 104 L72 134 L62 116" fill="none" stroke="url(#njGiDark)" stroke-width="16" stroke-linecap="round" stroke-linejoin="round"/>' +
      '<circle cx="61" cy="112" r="9" fill="#1b1b1b"/>' +
      '<path d="M102 165 L94 214 L90 252" fill="none" stroke="url(#njGiDark)" stroke-width="20" stroke-linecap="round" stroke-linejoin="round"/>' +
      '<rect x="80" y="238" width="20" height="8" rx="3" fill="#141414"/>' +
      '<ellipse cx="96" cy="261" rx="17" ry="8" fill="#141414"/>' +
      '<path d="M86 100 Q112 86 138 100 L135 170 Q112 178 89 170 Z" fill="' + GI + '"/>' +
      '<path d="M100 95 L112 124 L124 95" fill="none" stroke="#141414" stroke-width="5" stroke-linejoin="round"/>' +
      '<rect x="87" y="151" width="50" height="12" rx="3" fill="#141414"/>' +
      '<path d="M121 162 l6 18 M125 161 l14 13" stroke="#141414" stroke-width="5" stroke-linecap="round"/>' +
      '<g id="njKick">' +
        '<path d="M114 165 L122 214 L122 252" fill="none" stroke="' + GI + '" stroke-width="20" stroke-linecap="round" stroke-linejoin="round"/>' +
        '<rect x="112" y="238" width="20" height="8" rx="3" fill="#141414"/>' +
        '<ellipse cx="130" cy="261" rx="17" ry="8" fill="#141414"/>' +
      '</g>' +
      '<path d="M130 104 L154 122 L174 110" fill="none" stroke="' + GI + '" stroke-width="16" stroke-linecap="round" stroke-linejoin="round"/>' +
      '<circle cx="176" cy="108" r="9" fill="#1b1b1b"/>' +
      '<circle cx="112" cy="68" r="28" fill="' + GI + '"/>' +
      '<path d="M86 50 Q112 36 139 52 L140 60 Q112 46 86 58 Z" fill="#141414"/>' +
      '<path d="M98 62 Q120 56 140 63 L140 78 Q120 74 98 78 Z" fill="#F1C48A"/>' +
      '<ellipse cx="115" cy="69.5" rx="5.2" ry="4" fill="#fff"/><circle cx="117.5" cy="69.5" r="2.5" fill="#111"/>' +
      '<ellipse cx="132" cy="68.5" rx="4.6" ry="3.8" fill="#fff"/><circle cx="134.3" cy="68.5" r="2.3" fill="#111"/>' +
      '<path d="M108 63 L122 66 M127 65.5 L139 62.5" stroke="#111" stroke-width="3" stroke-linecap="round"/>' +
    '</g>' +
    '<g id="njBallRest" transform="translate(174 251) scale(.27)">' + ballMarkup('R', false) + '</g>' +
    '</svg>';

  /* ---------- DOM ---------- */
  var root = document.createElement('div');
  root.id = 'ninja-intro';
  root.setAttribute('role', 'presentation');
  root.innerHTML =
    '<div class="nj-stage">' +
      '<canvas class="nj-dragon"></canvas>' +
      '<div class="nj-hero">' + ninjaSvg + '</div>' +
      '<div class="nj-ball-fly"><svg viewBox="-50 -50 100 100">' + ballMarkup('F', true) + '</svg></div>' +
      '<div class="nj-burn"></div>' +
      '<canvas class="nj-canvas"></canvas>' +
      '<div class="nj-flash"></div>' +
    '</div>' +
    '<button type="button" class="nj-skip">Geç ›</button>';

  var prevOverflow = document.documentElement.style.overflow;
  document.documentElement.style.overflow = 'hidden';
  document.body.insertBefore(root, document.body.firstChild);

  var stage = root.querySelector('.nj-stage');
  var hero = root.querySelector('.nj-hero');
  var canvas = root.querySelector('.nj-canvas');
  var ctx = canvas.getContext('2d');
  var dCanvas = root.querySelector('.nj-dragon');
  var dctx = dCanvas.getContext('2d');
  var flyBall = root.querySelector('.nj-ball-fly');
  var flash = root.querySelector('.nj-flash');
  var burn = root.querySelector('.nj-burn');
  var body = root.querySelector('#njBody');
  var tails = root.querySelector('#njTails');
  var kick = root.querySelector('#njKick');
  var restBall = root.querySelector('#njBallRest');

  /* ---------- Canvas boyutları ---------- */
  var W = 0, H = 0, SF = 1, FS = .5;      // ateş canvas'ı düşük çözünürlükte (bulanık ateş zaten)
  var DDPR = 1;                           // ejderha canvas'ı
  function resize() {
    W = window.innerWidth; H = window.innerHeight;
    SF = Math.max(.55, Math.min(W, H) / 800);
    canvas.width = Math.round(W * FS);
    canvas.height = Math.round(H * FS);
    ctx.setTransform(FS, 0, 0, FS, 0, 0);

    DDPR = Math.min(window.devicePixelRatio || 1, 1.5);
    dCanvas.width = Math.round(W * DDPR);
    dCanvas.height = Math.round(H * DDPR);
    dctx.setTransform(DDPR, 0, 0, DDPR, 0, 0);
    buildPath();
  }

  function sprite(r, g, b) {
    var s = document.createElement('canvas');
    s.width = s.height = 64;
    var x = s.getContext('2d');
    var gr = x.createRadialGradient(32, 32, 0, 32, 32, 32);
    gr.addColorStop(0, 'rgba(' + r + ',' + g + ',' + b + ',1)');
    gr.addColorStop(.4, 'rgba(' + r + ',' + g + ',' + b + ',.55)');
    gr.addColorStop(1, 'rgba(' + r + ',' + g + ',' + b + ',0)');
    x.fillStyle = gr;
    x.fillRect(0, 0, 64, 64);
    return s;
  }
  var SPR = {
    white: sprite(255, 250, 225),
    yellow: sprite(255, 205, 60),
    orange: sprite(255, 120, 25),
    red: sprite(215, 45, 15),
    smoke: sprite(55, 42, 40)
  };
  // ekranı kaplayan ateş: beyaza patlamasın diye sarı-turuncu-kırmızı
  function warmSprite(t) { return t < .2 ? SPR.yellow : t < .5 ? SPR.orange : SPR.red; }
  function fireSprite(t) { return t < .12 ? SPR.white : t < .3 ? SPR.yellow : t < .55 ? SPR.orange : SPR.red; }

  /* =========================================================
     KIRMIZI EJDERHA
     ========================================================= */
  var DN = 64;                 // gövde parçası sayısı
  var dBreath = [];            // ağızdan çıkan alev
  var dBodyPts = [];

  /* Uçuş yolu: sağ üst köşe -> ninjanın etrafında S çizerek bir tur -> sol üst köşe */
  var PATH = [], PLEN = 0, DRAGON_DUR = 3400;
  function buildPath() {
    var nxC = hero.offsetLeft + hero.offsetWidth * .5;
    var nyC = hero.offsetTop + hero.offsetHeight * .5;
    var rx = hero.offsetWidth * 1.0, ry = hero.offsetHeight * .85;
    var ctrl = [
      [W * 1.25, -H * .3],
      [W * 1.05, -H * .08],
      [W * .72, H * .2],
      [nxC + rx * 1.9, nyC - ry * 1.1]
    ];
    // ninjanın etrafında tur (sağ -> alt -> sol -> üst -> sağ)
    for (var a = -20; a <= 330; a += 35) {
      var rad = a * PI / 180;
      ctrl.push([nxC + Math.cos(rad) * rx, nyC + Math.sin(rad) * ry]);
    }
    ctrl.push([nxC + rx * .2, nyC - ry * 1.9]);
    ctrl.push([W * .28, H * .12]);
    ctrl.push([-W * .05, -H * .1]);
    ctrl.push([-W * .3, -H * .35]);

    // Catmull-Rom ile pürüzsüz yol
    PATH = [];
    var i, j, steps = 24;
    for (i = 0; i < ctrl.length - 1; i++) {
      var p0 = ctrl[Math.max(0, i - 1)], p1 = ctrl[i], p2 = ctrl[i + 1], p3 = ctrl[Math.min(ctrl.length - 1, i + 2)];
      for (j = 0; j < steps; j++) {
        var u = j / steps, u2 = u * u, u3 = u2 * u;
        PATH.push({
          x: .5 * (2 * p1[0] + (-p0[0] + p2[0]) * u + (2 * p0[0] - 5 * p1[0] + 4 * p2[0] - p3[0]) * u2 + (-p0[0] + 3 * p1[0] - 3 * p2[0] + p3[0]) * u3),
          y: .5 * (2 * p1[1] + (-p0[1] + p2[1]) * u + (2 * p0[1] - 5 * p1[1] + 4 * p2[1] - p3[1]) * u2 + (-p0[1] + 3 * p1[1] - 3 * p2[1] + p3[1]) * u3)
        });
      }
    }
    var lp = ctrl[ctrl.length - 1];
    PATH.push({ x: lp[0], y: lp[1] });
    PATH[0].d = 0;
    for (i = 1; i < PATH.length; i++) PATH[i].d = PATH[i - 1].d + Math.hypot(PATH[i].x - PATH[i - 1].x, PATH[i].y - PATH[i - 1].y);
    PLEN = PATH[PATH.length - 1].d;
  }
  function pathAt(s) {
    var n = PATH.length, a, b;
    if (s <= 0) { a = PATH[0]; b = PATH[1]; }
    else if (s >= PLEN) { a = PATH[n - 2]; b = PATH[n - 1]; }
    else {
      var lo = 0, hi = n - 1;
      while (hi - lo > 1) { var m = (lo + hi) >> 1; if (PATH[m].d < s) lo = m; else hi = m; }
      a = PATH[lo]; b = PATH[hi];
    }
    var segL = (b.d - a.d) || 1, u = (s - a.d) / segL;
    return { x: a.x + (b.x - a.x) * u, y: a.y + (b.y - a.y) * u };
  }

  function segRadius(i, R) {
    var s = i / (DN - 1);
    if (s < .07) return R * (.72 + .28 * s / .07);
    return R * (.1 + .9 * Math.pow(1 - (s - .07) / .93, .85));
  }

  function drawDragon(t, k, alpha) {
    var g = dctx;
    g.clearRect(0, 0, W, H);
    if (alpha <= 0) return;
    var R = Math.max(10, Math.min(W, H) * .035);
    var seg = R * .46, pts = dBodyPts, bodyLen = seg * DN;
    var sHead = (t / DRAGON_DUR) * (PLEN + bodyLen + R * 4);
    if (sHead - bodyLen > PLEN + R * 4) return;   // ejderha ekrandan çıktı
    for (var si = 0; si < DN; si++) {
      var bp = pathAt(sHead - si * seg);
      bp.x += Math.sin(t / 150 - si * .35) * R * .12;   // hafif yılan dalgası
      pts[si] = bp;
    }
    var i, p, d, nx, ny, r, len;
    var dirs = [];
    for (i = 0; i < DN; i++) {
      var a = pts[Math.max(0, i - 1)], b = pts[Math.min(DN - 1, i + (i === 0 ? 1 : 0))];
      d = i === 0 ? { x: pts[0].x - pts[1].x, y: pts[0].y - pts[1].y } : { x: a.x - pts[i].x, y: a.y - pts[i].y };
      len = Math.hypot(d.x, d.y) || 1;
      dirs.push({ x: d.x / len, y: d.y / len });
    }

    g.save();
    g.globalAlpha = alpha;
    g.lineCap = 'round';
    g.lineJoin = 'round';

    /* Bacaklar (gövdenin altında kalır) */
    var legAt = [10, 30];
    for (var L = 0; L < legAt.length; L++) {
      i = legAt[L]; p = pts[i]; d = dirs[i]; nx = -d.y; ny = d.x; r = segRadius(i, R);
      for (var side = -1; side <= 1; side += 2) {
        var ph = Math.sin(t / 180 + L * 1.7 + (side > 0 ? 0 : PI)) * .5;
        var sx = p.x + nx * side * r * .35, sy = p.y + ny * side * r * .35;
        var ex = sx + nx * side * r * 1.5 - d.x * r * (.6 + ph), ey = sy + ny * side * r * 1.5 - d.y * r * (.6 + ph);
        var fx = ex + d.x * r * (1.1 + ph * .6) + nx * side * r * .5, fy = ey + d.y * r * (1.1 + ph * .6) + ny * side * r * .5;
        g.strokeStyle = side > 0 ? '#8c0b12' : '#5a060b';
        g.lineWidth = r * .75;
        g.beginPath(); g.moveTo(sx, sy); g.lineTo(ex, ey); g.stroke();
        g.lineWidth = r * .5;
        g.beginPath(); g.moveTo(ex, ey); g.lineTo(fx, fy); g.stroke();
        // pençeler
        g.strokeStyle = '#efe2c4';
        g.lineWidth = Math.max(1, r * .12);
        for (var c = -1; c <= 1; c++) {
          var ca = Math.atan2(fy - ey, fx - ex) + c * .55;
          g.beginPath();
          g.moveTo(fx, fy);
          g.quadraticCurveTo(fx + Math.cos(ca) * r * .45, fy + Math.sin(ca) * r * .45, fx + Math.cos(ca + .5 * side) * r * .6, fy + Math.sin(ca + .5 * side) * r * .6);
          g.stroke();
        }
      }
    }

    /* Kuyruk yelpazesi */
    p = pts[DN - 1]; d = dirs[DN - 1];
    var ta = Math.atan2(-d.y, -d.x);
    for (i = -2; i <= 2; i++) {
      var fa = ta + i * .28 + Math.sin(t / 140 + i) * .08;
      var fl = R * (1.6 - Math.abs(i) * .25);
      g.fillStyle = i % 2 ? '#7a0710' : '#a50e17';
      g.beginPath();
      g.moveTo(p.x + Math.cos(fa + PI / 2) * R * .12, p.y + Math.sin(fa + PI / 2) * R * .12);
      g.quadraticCurveTo(p.x + Math.cos(fa + .25) * fl * .6, p.y + Math.sin(fa + .25) * fl * .6, p.x + Math.cos(fa) * fl, p.y + Math.sin(fa) * fl);
      g.quadraticCurveTo(p.x + Math.cos(fa - .25) * fl * .6, p.y + Math.sin(fa - .25) * fl * .6, p.x + Math.cos(fa - PI / 2) * R * .12, p.y + Math.sin(fa - PI / 2) * R * .12);
      g.fill();
    }

    /* Gövde: kuyruktan başa */
    for (i = DN - 1; i >= 1; i--) {
      p = pts[i]; d = dirs[i]; nx = -d.y; ny = d.x; r = segRadius(i, R);

      // sırt dikenleri
      if (i % 2 === 0 && i < DN - 3) {
        var sh = r * (.55 + .35 * Math.sin(i * .9)) * (i < 6 ? .6 : 1);
        var bx = p.x - nx * r * .7, by = p.y - ny * r * .7;
        g.fillStyle = '#3e0207';
        g.beginPath();
        g.moveTo(bx + d.x * r * .45, by + d.y * r * .45);
        g.lineTo(bx - nx * sh - d.x * r * .75, by - ny * sh - d.y * r * .75);
        g.lineTo(bx - d.x * r * .45, by - d.y * r * .45);
        g.closePath();
        g.fill();
      }

      // pullu gövde parçası
      var gr = g.createRadialGradient(p.x - r * .35, p.y - r * .45, r * .1, p.x, p.y, r * 1.05);
      gr.addColorStop(0, '#ff5a45');
      gr.addColorStop(.45, '#c4131c');
      gr.addColorStop(1, '#42030a');
      g.fillStyle = gr;
      g.beginPath(); g.arc(p.x, p.y, r, 0, TAU); g.fill();

      // pul kenarı (kuyruğa bakan taraf)
      var da = Math.atan2(d.y, d.x);
      g.strokeStyle = 'rgba(40,0,4,.6)';
      g.lineWidth = Math.max(.8, r * .14);
      g.beginPath(); g.arc(p.x, p.y, r * .9, da + PI - 1.05, da + PI + 1.05); g.stroke();
      g.strokeStyle = 'rgba(255,140,110,.28)';
      g.lineWidth = Math.max(.6, r * .07);
      g.beginPath(); g.arc(p.x, p.y, r * .7, da + PI - .8, da + PI - .1); g.stroke();

      // karın plakası
      if (i < DN - 4) {
        g.save();
        g.translate(p.x + nx * r * .6, p.y + ny * r * .6);
        g.rotate(da);
        g.fillStyle = '#d99b3c';
        g.strokeStyle = '#6e3a0a';
        g.lineWidth = Math.max(.6, r * .08);
        g.beginPath(); g.ellipse(0, 0, r * .42, r * .3, 0, 0, TAU); g.fill(); g.stroke();
        g.restore();
      }
    }

    /* Baş */
    p = pts[0]; d = dirs[0];
    var th = Math.atan2(d.y, d.x);
    var flip = Math.cos(th) < 0 ? -1 : 1;
    var HS = R * 3.1;
    var hx = p.x + d.x * R * .3, hy = p.y + d.y * R * .3;
    g.save();
    g.translate(hx, hy);
    g.rotate(th);
    g.scale(1, flip);

    // yele
    for (i = -3; i <= 3; i++) {
      var ma = PI + i * .32 + Math.sin(t / 120 + i) * .07;
      var ml = HS * (.8 - Math.abs(i) * .07);
      g.fillStyle = i % 2 ? '#5e0309' : '#8e0a12';
      g.beginPath();
      g.moveTo(-HS * .05 + Math.cos(ma + PI / 2) * HS * .14, Math.sin(ma + PI / 2) * HS * .14);
      g.quadraticCurveTo(Math.cos(ma + .2) * ml * .6, Math.sin(ma + .2) * ml * .6, Math.cos(ma) * ml, Math.sin(ma) * ml);
      g.quadraticCurveTo(Math.cos(ma - .15) * ml * .5, Math.sin(ma - .15) * ml * .5, -HS * .05 + Math.cos(ma - PI / 2) * HS * .14, Math.sin(ma - PI / 2) * HS * .14);
      g.fill();
    }

    // boynuzlar (arka + ön)
    function horn(ox, oy, col1, col2, s) {
      var hg = g.createLinearGradient(ox, oy, ox - HS * .9 * s, oy - HS * .55 * s);
      hg.addColorStop(0, col1); hg.addColorStop(1, col2);
      g.fillStyle = hg;
      g.beginPath();
      g.moveTo(ox + HS * .07, oy);
      g.quadraticCurveTo(ox - HS * .3 * s, oy - HS * .62 * s, ox - HS * .92 * s, oy - HS * .5 * s);
      g.quadraticCurveTo(ox - HS * .32 * s, oy - HS * .42 * s, ox - HS * .08, oy + HS * .04);
      g.closePath();
      g.fill();
    }
    horn(HS * .02, -HS * .24, '#bfa77a', '#5d4a2c', .85);
    horn(HS * .1, -HS * .28, '#f6e7c4', '#8a7450', 1);

    // ağız içi
    var jaw = .14 + .1 * Math.max(0, Math.sin(t / 380));
    g.fillStyle = '#2a0002';
    g.beginPath();
    g.moveTo(-HS * .15, HS * .1);
    g.lineTo(HS * .95, HS * .1);
    g.lineTo(HS * .85, HS * .1 + Math.sin(jaw) * HS * .9);
    g.closePath();
    g.fill();

    // alt çene
    g.save();
    g.translate(-HS * .1, HS * .12);
    g.rotate(jaw);
    var jg = g.createLinearGradient(0, 0, 0, HS * .2);
    jg.addColorStop(0, '#a50d15'); jg.addColorStop(1, '#4a0308');
    g.fillStyle = jg;
    g.beginPath();
    g.moveTo(-HS * .1, -HS * .02);
    g.lineTo(HS * .85, HS * .02);
    g.quadraticCurveTo(HS * .9, HS * .08, HS * .78, HS * .12);
    g.quadraticCurveTo(HS * .3, HS * .2, -HS * .12, HS * .15);
    g.closePath();
    g.fill();
    g.fillStyle = '#f4ecd6';
    for (i = 0; i < 6; i++) {
      var tx = HS * (.2 + i * .11);
      g.beginPath(); g.moveTo(tx, HS * .01); g.lineTo(tx + HS * .03, -HS * .07); g.lineTo(tx + HS * .06, HS * .015); g.fill();
    }
    g.restore();

    // üst kafa
    var hgUp = g.createLinearGradient(0, -HS * .42, 0, HS * .14);
    hgUp.addColorStop(0, '#ff4b38'); hgUp.addColorStop(.5, '#b80f18'); hgUp.addColorStop(1, '#4e0309');
    g.fillStyle = hgUp;
    g.beginPath();
    g.moveTo(-HS * .32, -HS * .2);
    g.bezierCurveTo(-HS * .12, -HS * .44, HS * .24, -HS * .42, HS * .38, -HS * .3);
    g.bezierCurveTo(HS * .55, -HS * .2, HS * .84, -HS * .19, HS * 1.0, -HS * .06);
    g.quadraticCurveTo(HS * 1.07, HS * .04, HS * .96, HS * .11);
    g.lineTo(HS * .2, HS * .12);
    g.quadraticCurveTo(-HS * .1, HS * .22, -HS * .32, HS * .2);
    g.closePath();
    g.fill();

    // üst dişler
    g.fillStyle = '#f7f0dc';
    for (i = 0; i < 7; i++) {
      var ux = HS * (.25 + i * .1);
      var tl = i === 1 ? .12 : .07;
      g.beginPath(); g.moveTo(ux, HS * .1); g.lineTo(ux + HS * .025, HS * (.1 + tl)); g.lineTo(ux + HS * .05, HS * .1); g.fill();
    }

    // kafa pulları / çizgiler
    g.strokeStyle = 'rgba(50,0,5,.55)';
    g.lineWidth = Math.max(.8, HS * .018);
    for (i = 0; i < 4; i++) {
      g.beginPath(); g.arc(HS * (.05 + i * .17), -HS * .05, HS * .14, PI * 1.1, PI * 1.6); g.stroke();
    }
    // kaş çıkıntısı
    g.strokeStyle = '#330106';
    g.lineWidth = HS * .055;
    g.beginPath(); g.moveTo(HS * .17, -HS * .31); g.quadraticCurveTo(HS * .36, -HS * .4, HS * .52, -HS * .24); g.stroke();

    // göz
    g.save();
    g.shadowColor = 'rgba(255,190,0,.9)';
    g.shadowBlur = HS * .15;
    var eg = g.createRadialGradient(HS * .36, -HS * .2, 0, HS * .36, -HS * .2, HS * .075);
    eg.addColorStop(0, '#fff7b0'); eg.addColorStop(.5, '#ffb300'); eg.addColorStop(1, '#ff4a00');
    g.fillStyle = eg;
    g.beginPath(); g.ellipse(HS * .36, -HS * .2, HS * .075, HS * .045, -.15, 0, TAU); g.fill();
    g.restore();
    g.fillStyle = '#120000';
    g.beginPath(); g.ellipse(HS * .37, -HS * .2, HS * .013, HS * .04, 0, 0, TAU); g.fill();

    // burun deliği
    g.fillStyle = '#250003';
    g.beginPath(); g.ellipse(HS * .9, -HS * .04, HS * .035, HS * .02, -.4, 0, TAU); g.fill();

    // yanak dikenleri
    g.fillStyle = '#5a0309';
    for (i = 0; i < 3; i++) {
      g.beginPath();
      g.moveTo(-HS * .2 + i * HS * .08, HS * .16);
      g.lineTo(-HS * .45 + i * HS * .05, HS * (.36 + i * .05));
      g.lineTo(-HS * .1 + i * HS * .08, HS * .2);
      g.fill();
    }

    // bıyıklar
    g.strokeStyle = '#f0c25a';
    g.lineWidth = Math.max(1, HS * .024);
    for (i = 0; i < 2; i++) {
      var wv = Math.sin(t / 160 + i * 1.3);
      g.beginPath();
      g.moveTo(HS * .86, HS * (i ? -.02 : .06));
      g.bezierCurveTo(HS * .5, HS * (.45 + wv * .12), -HS * .2, HS * (.2 - wv * .2), -HS * (.95 + i * .2), HS * (.55 + wv * .25 + i * .1));
      g.stroke();
    }
    g.restore();

    /* Ağızdan alev nefesi */
    var breathing = Math.sin(t / 380) > .2;
    if (breathing) {
      var mx = hx + d.x * HS * .95 + (-d.y) * HS * .14 * flip;
      var my = hy + d.y * HS * .95 + (d.x) * HS * .14 * flip;
      for (i = 0; i < Math.round(3 * k); i++) {
        var sp = rnd(2, 4.5) * SF;
        var ang = th + rnd(-.25, .25) + jaw * .5 * flip;
        dBreath.push({ x: mx, y: my, vx: Math.cos(ang) * sp, vy: Math.sin(ang) * sp, s: R * rnd(.5, .9), age: 0, life: rnd(16, 28) });
      }
    }
    g.globalCompositeOperation = 'lighter';
    for (i = dBreath.length - 1; i >= 0; i--) {
      var b2 = dBreath[i];
      b2.age += k;
      var bt = b2.age / b2.life;
      if (bt >= 1) { dBreath.splice(i, 1); continue; }
      b2.x += b2.vx * k; b2.y += b2.vy * k - .15 * k;
      var br2 = b2.s * (.6 + bt * 1.6);
      g.globalAlpha = alpha * (1 - bt) * .9;
      g.drawImage(fireSprite(bt), b2.x - br2, b2.y - br2, br2 * 2, br2 * 2);
    }
    g.restore();
  }

  resize();
  window.addEventListener('resize', resize);

  /* =========================================================
     ATEŞ PARÇACIKLARI (top izi + patlama + ekranı kaplayan ateş)
     ========================================================= */
  var parts = [], embers = [], rings = [];

  function addFire(x, y, vx, vy, size, life, lift, drag, alpha, warm) {
    if (parts.length > 750) return;
    parts.push({ x: x, y: y, vx: vx, vy: vy, s: size, life: life, age: 0, lift: lift, drag: drag, a: alpha || 1, w: !!warm });
  }

  function updateDraw(k) {
    ctx.clearRect(0, 0, W, H);
    var i, p, t, r;

    ctx.globalCompositeOperation = 'source-over';
    for (i = 0; i < parts.length; i++) {
      p = parts[i];
      t = p.age / p.life;
      if (t > .7) {
        r = p.s * (0.7 + t * 1.1);
        ctx.globalAlpha = .4 * (1 - t) / .3;
        ctx.drawImage(SPR.smoke, p.x - r, p.y - r, r * 2, r * 2);
      }
    }

    ctx.globalCompositeOperation = 'lighter';
    for (i = parts.length - 1; i >= 0; i--) {
      p = parts[i];
      p.age += k;
      t = p.age / p.life;
      if (t >= 1) { parts.splice(i, 1); continue; }
      p.vx *= Math.pow(p.drag, k);
      p.vy = p.vy * Math.pow(p.drag, k) + p.lift * k;
      p.x += p.vx * k;
      p.y += p.vy * k;
      if (t > .82) continue;
      r = p.s * (0.55 + t * 0.9);
      ctx.globalAlpha = Math.pow(1 - t, 1.1) * p.a;
      ctx.drawImage(p.w ? warmSprite(t) : fireSprite(t), p.x - r, p.y - r, r * 2, r * 2);
    }

    ctx.lineCap = 'round';
    for (i = embers.length - 1; i >= 0; i--) {
      p = embers[i];
      p.age += k;
      t = p.age / p.life;
      if (t >= 1) { embers.splice(i, 1); continue; }
      var px = p.x, py = p.y;
      p.vx *= Math.pow(.97, k);
      p.vy = p.vy * Math.pow(.97, k) + p.g * k;
      p.x += p.vx * k;
      p.y += p.vy * k;
      ctx.globalAlpha = 1 - t;
      ctx.strokeStyle = t < .4 ? '#FFF2B0' : '#FFA42A';
      ctx.lineWidth = p.w;
      ctx.beginPath();
      ctx.moveTo(px - p.vx * 1.5 * k, py - p.vy * 1.5 * k);
      ctx.lineTo(p.x, p.y);
      ctx.stroke();
    }

    for (i = rings.length - 1; i >= 0; i--) {
      p = rings[i];
      p.age += k;
      t = p.age / p.life;
      if (t >= 1) { rings.splice(i, 1); continue; }
      if (t <= 0) continue;
      ctx.globalAlpha = (1 - t) * .85;
      ctx.strokeStyle = '#FFB13A';
      ctx.lineWidth = Math.max(1, 30 * SF * (1 - t));
      ctx.beginPath();
      ctx.arc(p.x, p.y, p.max * easeOut(t), 0, TAU);
      ctx.stroke();
    }

    ctx.globalAlpha = 1;
    ctx.globalCompositeOperation = 'source-over';
  }

  /* ---------- Çarpma: tüm ekran ateş ---------- */
  var COVER = 1500;          // ekranın ateşle kaplı kaldığı süre (ms)
  var coverStart = 0, boom = null;

  function impact(x, y, now) {
    boom = { x: x, y: y };
    coverStart = now;
    var i, ang, sp, big = Math.max(W, H);
    for (i = 0; i < 120; i++) {
      ang = Math.random() * TAU;
      sp = rnd(4, 26) * SF;
      addFire(x, y, Math.cos(ang) * sp, Math.sin(ang) * sp * .85, rnd(.05, .13) * big, rnd(40, 75), -0.12 * SF, .93, .7);
    }
    for (i = 0; i < 140; i++) {
      ang = Math.random() * TAU;
      sp = rnd(9, 30) * SF;
      embers.push({ x: x, y: y, vx: Math.cos(ang) * sp, vy: Math.sin(ang) * sp - 3 * SF, g: .2 * SF, w: rnd(1.5, 4) * SF, life: rnd(45, 100), age: 0 });
    }
    rings.push({ x: x, y: y, max: big * .9, life: 30, age: 0 });
    rings.push({ x: x, y: y, max: big * .55, life: 24, age: -5 });

    flash.style.setProperty('--fx', (x / W * 100).toFixed(1) + '%');
    flash.style.setProperty('--fy', (y / H * 100).toFixed(1) + '%');
    flash.classList.add('on');
    burn.classList.add('on');
    stage.classList.add('nj-shake');
  }

  function coverFire(now, k) {
    var e = now - coverStart;
    if (!boom || e > COVER) return;
    var big = Math.max(W, H);
    var power = e < 900 ? 1 : 1 - (e - 900) / (COVER - 900);
    var i, n;
    // ekranın her yerinde alev dilleri
    n = Math.round(9 * k * power);
    for (i = 0; i < n; i++) {
      addFire(rnd(-.05, 1.05) * W, rnd(.05, 1.1) * H, rnd(-1.2, 1.2) * SF, -rnd(3, 8) * SF,
        rnd(.09, .2) * big, rnd(35, 60), -0.2 * SF, .95, .42, true);
    }
    // alttan yükselen ateş duvarı
    n = Math.round(7 * k * power);
    for (i = 0; i < n; i++) {
      addFire(rnd(0, 1) * W, H + rnd(0, .05) * H, rnd(-1, 1) * SF, -rnd(8, 18) * SF,
        rnd(.12, .22) * big, rnd(40, 70), -0.25 * SF, .96, .5, true);
    }
    // çarpma noktasından yükselen alev
    n = Math.round(4 * k * power);
    for (i = 0; i < n; i++) {
      addFire(boom.x + rnd(-.12, .12) * W, boom.y + rnd(-.05, .08) * H, rnd(-1.5, 1.5) * SF, -rnd(6, 12) * SF,
        rnd(.07, .14) * big, rnd(30, 50), -0.22 * SF, .95, .6, true);
    }
  }

  /* ---------- Zaman çizelgesi ---------- */
  var T_WIND = 1300, T_KICK = 1600, T_KICK_END = 1830, T_HOLD = 2200, T_BACK = 2600, FLIGHT = 720;

  function legAngle(t) {
    var p;
    if (t < T_WIND) return 0;
    if (t < T_KICK) { p = (t - T_WIND) / (T_KICK - T_WIND); return 40 * easeOut(p); }
    if (t < T_KICK_END) { p = (t - T_KICK) / (T_KICK_END - T_KICK); return 40 - 145 * p * p; }
    if (t < T_HOLD) return -105;
    if (t < T_BACK) { p = (t - T_HOLD) / (T_BACK - T_HOLD); return -105 + 105 * easeInOut(p); }
    return 0;
  }

  var t0 = performance.now(), last = t0, raf = 0, done = false;
  var launched = false, launchT = 0, exploded = false, explodeT = 0;
  var fl = null, prevBall = null;

  function launch(now) {
    launched = true;
    launchT = now;
    var r = restBall.getBoundingClientRect();
    restBall.style.visibility = 'hidden';
    var D = Math.min(W, H) * 0.6;
    var sx = r.left + r.width / 2, sy = r.top + r.height / 2;
    var ex = W / 2, ey = H * 0.46;
    fl = {
      sx: sx, sy: sy, ex: ex, ey: ey,
      cx: (sx + ex) / 2, cy: Math.min(sy, ey) - H * 0.22,
      D: D, s0: Math.max(r.width, 4) / D
    };
    flyBall.style.width = D + 'px';
    flyBall.style.height = D + 'px';
    flyBall.style.display = 'block';
    for (var i = 0; i < 30; i++) {
      var ang = rnd(-1.6, .4), sp = rnd(3, 11) * SF;
      embers.push({ x: sx, y: sy, vx: Math.cos(ang) * sp, vy: Math.sin(ang) * sp, g: .25 * SF, w: rnd(1.2, 2.6) * SF, life: rnd(20, 40), age: 0 });
    }
    // vuruş anında topu tutuşturan alev
    for (i = 0; i < 18; i++) addFire(sx, sy, rnd(-2, 2) * SF, -rnd(1, 4) * SF, r.width * rnd(1, 2), rnd(14, 24), -.1 * SF, .94);
  }

  function loop(now) {
    if (done && !root.parentNode) return;
    var t = now - t0;
    var dt = Math.min(50, now - last);
    last = now;
    var k = dt / 16.667;

    /* Ejderha */
    drawDragon(t, k, 1);

    /* Ninja */
    var bob = t < T_WIND ? Math.sin(t / 210) * 2.2 : 0;
    body.setAttribute('transform', 'translate(0 ' + bob.toFixed(2) + ')');
    tails.setAttribute('transform', 'rotate(' + (Math.sin(t / 85) * 7).toFixed(2) + ' 86 56)');
    var ang = legAngle(t);
    kick.setAttribute('transform', 'rotate(' + ang.toFixed(2) + ' 112 165)');

    if (!launched && t > T_KICK && ang <= -20) launch(now);

    /* Alevli top ekrana doğru geliyor */
    if (launched && !exploded && fl) {
      var p = clamp01((now - launchT) / FLIGHT);
      var q = easeOut(p * 0.85 + p * p * 0.15);
      var bx = (1 - q) * (1 - q) * fl.sx + 2 * (1 - q) * q * fl.cx + q * q * fl.ex;
      var by = (1 - q) * (1 - q) * fl.sy + 2 * (1 - q) * q * fl.cy + q * q * fl.ey;
      var sc = fl.s0 + (1 - fl.s0) * Math.pow(p, 2.4);
      flyBall.style.transform = 'translate(' + (bx - fl.D / 2).toFixed(1) + 'px,' + (by - fl.D / 2).toFixed(1) + 'px) scale(' + sc.toFixed(3) + ') rotate(' + (p * 900).toFixed(1) + 'deg)';

      var br = fl.D * sc / 2;
      var mvx = prevBall ? bx - prevBall.x : 0, mvy = prevBall ? by - prevBall.y : 0;
      var ml = Math.hypot(mvx, mvy) || 1, ux = mvx / ml, uy = mvy / ml;
      prevBall = { x: bx, y: by };
      // topun etrafını saran alev + arkasında uzun alev kuyruğu
      var n = Math.round(14 * k);
      for (var e = 0; e < n; e++) {
        var a2 = Math.random() * TAU, rr = br * rnd(.5, 1.05);
        var px2 = bx + Math.cos(a2) * rr - ux * br * .35, py2 = by + Math.sin(a2) * rr - uy * br * .35;
        addFire(px2, py2,
          -ux * rnd(2, 7) * SF + Math.cos(a2) * 1.2 * SF, -uy * rnd(2, 7) * SF + Math.sin(a2) * 1.2 * SF - 1.2 * SF,
          Math.max(10, br * rnd(.35, .8)), rnd(12, 24), -0.18 * SF, .95, .9);
      }
      if (p >= 1) {
        exploded = true;
        explodeT = now;
        flyBall.style.display = 'none';
        impact(fl.ex, fl.ey, now);
      }
    }

    coverFire(now, k);
    updateDraw(k);

    if (exploded && now - explodeT > COVER + 200) finish();
    if (t > 10000) finish();

    raf = requestAnimationFrame(loop);
  }

  function finish() {
    if (done) return;
    done = true;
    markSeen();
    root.classList.add('nj-out');
    document.documentElement.style.overflow = prevOverflow;
    setTimeout(function () {
      cancelAnimationFrame(raf);
      window.removeEventListener('resize', resize);
      document.removeEventListener('keydown', onKey);
      if (root.parentNode) root.parentNode.removeChild(root);
    }, 800);
  }

  function onKey(ev) {
    if (ev.key === 'Escape' || ev.key === 'Enter' || ev.key === ' ') finish();
  }
  root.addEventListener('click', finish);
  document.addEventListener('keydown', onKey);

  raf = requestAnimationFrame(loop);
})();
