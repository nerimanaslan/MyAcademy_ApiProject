/* Serie A - Sıralama Yolculuğu (bump chart)
   X: oynanmış haftalar, Y: lig sırası (1. sıra üstte).
   20 takım soluk gri çizgi; seçili en fazla 3 takım renkli (renk takıma bağlıdır, sıraya değil).
   Etkileşim: çizgiye/tabloya tıkla = seç/bırak, üzerine gel = ipucu, hafta sütununda artı imleç. */
(function () {
  'use strict';

  // Doğrulanmış kategorik slotlar (koyu yüzey): mavi, turuncu, aqua
  const SLOTS = ['#3987e5', '#d95926', '#199e70'];
  const MAX_SELECTED = SLOTS.length;
  const NS = 'http://www.w3.org/2000/svg';

  let state = null;   // { data, selected: Map(teamId -> slotIndex) }

  function el(name, attrs, parent) {
    const node = document.createElementNS(NS, name);
    for (const k in attrs) node.setAttribute(k, attrs[k]);
    if (parent) parent.appendChild(node);
    return node;
  }
  const esc = (s) => String(s).replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

  function init() {
    const root = document.querySelector('[data-rank-chart]');
    const svg = root && root.querySelector('[data-rank-svg]');
    if (!root) return;
    const data = JSON.parse(root.querySelector('[data-rank-data]').textContent);
    if (!svg || data.weeks.length < 2) return;

    // Seçim canlı güncellemelerde korunur; ilk açılışta son haftanın ilk 3'ü
    const previous = state ? state.selected : null;
    const selected = new Map();
    const ids = new Set(data.teams.map((t) => t.id));
    if (previous) previous.forEach((slot, id) => { if (ids.has(id)) selected.set(id, slot); });
    if (!selected.size) data.teams.slice(0, MAX_SELECTED).forEach((t, i) => selected.set(t.id, i));

    state = { root, svg, data, selected, legend: root.querySelector('[data-rank-legend]'), tooltip: root.querySelector('[data-rank-tooltip]') };
    render();
  }

  function toggle(teamId) {
    const sel = state.selected;
    if (sel.has(teamId)) { sel.delete(teamId); }
    else {
      if (sel.size >= MAX_SELECTED) sel.delete(sel.keys().next().value);   // en eski seçimi bırak
      const used = new Set(sel.values());
      sel.set(teamId, [0, 1, 2].find((i) => !used.has(i)));
    }
    render();
  }

  function render() {
    const { svg, data, selected } = state;
    const weeks = data.weeks, teams = data.teams, n = weeks.length, count = teams.length;

    const width = Math.max(svg.parentElement.clientWidth, 320);
    const m = { top: 34, right: 118, bottom: 10, left: 40 };
    const rowH = width < 560 ? 18 : 22;
    const height = m.top + (count - 1) * rowH + m.bottom + 8;
    const plotW = width - m.left - m.right;
    const x = (i) => m.left + (n === 1 ? plotW / 2 : (i * plotW) / (n - 1));
    const y = (pos) => m.top + (pos - 1) * rowH;

    svg.setAttribute('viewBox', `0 0 ${width} ${height}`);
    svg.setAttribute('width', width);
    svg.setAttribute('height', height);
    svg.innerHTML = '';

    // Bölge bantları (ŞL 1-4, düşme son 3) - çok soluk, sadece bağlam
    el('rect', { class: 'rc-zone rc-zone--ucl', x: m.left - 8, y: y(1) - rowH / 2, width: plotW + 16, height: rowH * 4 }, svg);
    el('rect', { class: 'rc-zone rc-zone--rel', x: m.left - 8, y: y(count - 2) - rowH / 2, width: plotW + 16, height: rowH * 3 }, svg);

    // Seçili hafta sütunu (zaman makinesi ile aynı hafta)
    const selIndex = weeks.indexOf(data.selectedWeek);
    if (selIndex >= 0) el('rect', { class: 'rc-week-band', x: x(selIndex) - 14, y: m.top - 26, width: 28, height: height - m.top + 20, rx: 6 }, svg);

    // Eksenler: üstte hafta etiketleri, solda sıra numaraları
    weeks.forEach((w, i) => {
      const t = el('text', { class: 'rc-axis rc-axis--week' + (i === selIndex ? ' is-active' : ''), x: x(i), y: m.top - 14, 'text-anchor': 'middle' }, svg);
      t.textContent = `${w}. Hafta`;
    });
    [1, 5, 10, 15, count].forEach((p) => {
      const t = el('text', { class: 'rc-axis', x: m.left - 14, y: y(p) + 4, 'text-anchor': 'end' }, svg);
      t.textContent = p + '.';
    });

    const pathFor = (team) => team.positions.map((p, i) => `${i ? 'L' : 'M'}${x(i)},${y(p)}`).join(' ');

    // 1) Bağlam: tüm takımlar soluk
    const context = el('g', { class: 'rc-context' }, svg);
    teams.forEach((team) => {
      if (selected.has(team.id)) return;
      el('path', { class: 'rc-line', d: pathFor(team), 'data-team': team.id }, context);
    });

    // 2) Seçili takımlar: renkli çizgi + yüzey halkalı noktalar
    const focus = el('g', { class: 'rc-focus' }, svg);
    const labels = [];
    teams.filter((t) => selected.has(t.id)).forEach((team) => {
      const color = SLOTS[selected.get(team.id)];
      // style ile: CSS'teki .rc-line kuralı stroke özniteliğini ezer
      el('path', { class: 'rc-line rc-line--on', d: pathFor(team), style: `stroke:${color}`, 'data-team': team.id }, focus);
      team.positions.forEach((p, i) => el('circle', { class: 'rc-dot', cx: x(i), cy: y(p), r: 4.5, fill: color }, focus));
      labels.push({ team, color, y: y(team.positions[n - 1]) });
    });

    // Doğrudan etiketler (son haftanın sağında); çakışırsa aşağı kaydır
    labels.sort((a, b) => a.y - b.y);
    let lastY = -Infinity;
    labels.forEach((l) => {
      const ly = Math.max(l.y, lastY + 16); lastY = ly;
      const t = el('text', { class: 'rc-label', x: x(n - 1) + 12, y: ly + 4 }, focus);
      t.textContent = `${l.team.code} ${l.team.positions[n - 1]}.`;
    });

    // 3) Görünmez geniş tıklama/hover alanları (çizgiden büyük hedef)
    const hits = el('g', { class: 'rc-hits' }, svg);
    teams.forEach((team) => {
      const hit = el('path', { class: 'rc-hit', d: pathFor(team), 'data-team': team.id, tabindex: 0, role: 'button', 'aria-label': `${team.name}: grafikte ${selected.has(team.id) ? 'seçimi kaldır' : 'seç'}` }, hits);
      hit.addEventListener('click', () => toggle(team.id));
      hit.addEventListener('keydown', (e) => { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); toggle(team.id); } });
      hit.addEventListener('mouseenter', () => hoverTeam(team));
      hit.addEventListener('mouseleave', () => hoverTeam(null));
    });

    // 4) Artı imleç: en yakın hafta sütunu
    const cross = el('line', { class: 'rc-cross', x1: 0, x2: 0, y1: m.top - 6, y2: height - m.bottom, visibility: 'hidden' }, svg);
    svg.onmousemove = (e) => {
      const rect = svg.getBoundingClientRect();
      const px = (e.clientX - rect.left) * (width / rect.width);
      const i = Math.max(0, Math.min(n - 1, Math.round(((px - m.left) / plotW) * (n - 1))));
      cross.setAttribute('x1', x(i)); cross.setAttribute('x2', x(i)); cross.setAttribute('visibility', 'visible');
      if (!state.hovered) showWeekTip(i, e);
      else showTeamTip(state.hovered, e);
    };
    svg.onmouseleave = () => { cross.setAttribute('visibility', 'hidden'); state.tooltip.hidden = true; };

    renderLegend();
    syncTable();
  }

  function hoverTeam(team) {
    state.hovered = team;
    state.svg.querySelectorAll('.rc-context .rc-line').forEach((p) =>
      p.classList.toggle('is-hover', team && Number(p.dataset.team) === team.id));
  }

  function placeTip(html, e) {
    const tip = state.tooltip, box = state.svg.parentElement.getBoundingClientRect();
    tip.innerHTML = html; tip.hidden = false;
    let left = e.clientX - box.left + 14, top = e.clientY - box.top + 14;
    if (left + tip.offsetWidth > box.width) left = e.clientX - box.left - tip.offsetWidth - 14;
    tip.style.left = left + 'px'; tip.style.top = top + 'px';
  }

  function showWeekTip(i, e) {
    const { data, selected } = state;
    const rows = data.teams.filter((t) => selected.has(t.id))
      .sort((a, b) => a.positions[i] - b.positions[i])
      .map((t) => `<li><i style="background:${SLOTS[selected.get(t.id)]}"></i>${esc(t.name)}<b>${t.positions[i]}. · ${t.points[i]} p</b></li>`).join('');
    placeTip(`<strong>${data.weeks[i]}. hafta sonu</strong><ul>${rows || '<li>Takım seçin</li>'}</ul>`, e);
  }

  function showTeamTip(team, e) {
    const { data } = state;
    const rows = data.weeks.map((w, i) => {
      const prev = i ? team.positions[i - 1] - team.positions[i] : 0;
      const move = prev > 0 ? ` <em class="up">▲${prev}</em>` : prev < 0 ? ` <em class="down">▼${-prev}</em>` : '';
      return `<li>${w}. hafta<b>${team.positions[i]}. · ${team.points[i]} p${move}</b></li>`;
    }).join('');
    placeTip(`<strong>${esc(team.name)}</strong><ul>${rows}</ul><small>Tıkla: ${state.selected.has(team.id) ? 'seçimi kaldır' : 'karşılaştırmaya ekle'}</small>`, e);
  }

  function renderLegend() {
    const { legend, data, selected } = state;
    if (!legend) return;
    const chips = data.teams.filter((t) => selected.has(t.id)).map((t) =>
      `<button type="button" class="rc-chip" data-rank-remove="${t.id}" title="Grafikten çıkar">
         <i style="background:${SLOTS[selected.get(t.id)]}"></i>${esc(t.name)}<span aria-hidden="true">✕</span>
       </button>`).join('');
    legend.innerHTML = chips + (selected.size < MAX_SELECTED ? `<span class="rc-chip rc-chip--hint">+ takım seç (${MAX_SELECTED - selected.size})</span>` : '');
    legend.querySelectorAll('[data-rank-remove]').forEach((b) => b.addEventListener('click', () => toggle(Number(b.dataset.rankRemove))));
  }

  // Tabloda seçili takımların satırına renk işareti
  function syncTable() {
    document.querySelectorAll('.standings-row').forEach((row) => {
      const id = Number(row.dataset.team);
      const slot = state.selected.get(id);
      row.style.setProperty('--rank-color', slot === undefined ? 'transparent' : SLOTS[slot]);
      row.classList.toggle('is-compared', slot !== undefined);
    });
  }

  // Tablodaki satıra tıklama da grafiğe ekler/çıkarır (takım linki hariç)
  document.addEventListener('click', (e) => {
    if (!state || e.target.closest('a')) return;
    const row = e.target.closest('.standings-row');
    if (row) toggle(Number(row.dataset.team));
  });

  let resizeTimer;
  window.addEventListener('resize', () => { clearTimeout(resizeTimer); resizeTimer = setTimeout(() => state && render(), 150); });

  init();
  document.addEventListener('serie:content-updated', init);
})();
