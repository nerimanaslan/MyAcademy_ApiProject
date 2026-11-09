/* Serie A - Puan Durumu: bölge filtresi, takım arama, puan merdiveni seçimi */
(function () {
  'use strict';

  // Canlı güncellemede (live.js) <main> içeriği yenilenince tekrar bağlanır
  function init() {
    const root = document.querySelector('[data-standings]');
    if (!root) return;

    const rows = Array.from(root.querySelectorAll('.standings-row'));
    const markers = Array.from(root.querySelectorAll('[data-ladder-team]'));
    const chips = Array.from(root.querySelectorAll('[data-zone-filter]'));
    const search = root.querySelector('[data-team-search]');
    const counter = root.querySelector('[data-visible-count]');
    let activeZone = 'all';
    let selectedTeam = null;

    const normalize = (value) =>
      String(value || '').toLocaleLowerCase('tr-TR').normalize('NFD').replace(/[\u0300-\u036f]/g, '');

    function applyFilters() {
      const query = normalize(search ? search.value.trim() : '');
      let visible = 0;

      rows.forEach((row) => {
        const zoneMatch = activeZone === 'all' || row.dataset.zone === activeZone;
        const textMatch = !query || normalize(row.dataset.teamName).includes(query);
        row.hidden = !(zoneMatch && textMatch);
        if (!row.hidden) visible += 1;
      });

      if (counter) counter.textContent = String(visible);
    }

    function setZone(zone) {
      activeZone = zone;
      chips.forEach((chip) =>
        chip.setAttribute('aria-pressed', String(chip.dataset.zoneFilter === zone)));
      applyFilters();
    }

    chips.forEach((chip) => chip.addEventListener('click', () => setZone(chip.dataset.zoneFilter)));
    if (search) search.addEventListener('input', applyFilters);

    function selectTeam(teamId) {
      selectedTeam = selectedTeam === teamId ? null : teamId;
      rows.forEach((row) => row.classList.toggle('is-selected', row.dataset.team === selectedTeam));
      markers.forEach((marker) => {
        const on = marker.dataset.ladderTeam === selectedTeam;
        marker.classList.toggle('is-active', on);
        marker.setAttribute('aria-pressed', String(on));
      });

      if (!selectedTeam) return;
      const row = rows.find((r) => r.dataset.team === selectedTeam);
      if (!row) return;
      if (row.hidden) {
        if (search) search.value = '';
        setZone('all');
      }
      row.scrollIntoView({ block: 'center', behavior: 'smooth' });
    }

    rows.forEach((row) => row.addEventListener('click', () => selectTeam(row.dataset.team)));
    markers.forEach((marker) => marker.addEventListener('click', () => selectTeam(marker.dataset.ladderTeam)));
  }

  init();
  document.addEventListener('serie:content-updated', init);
})();
