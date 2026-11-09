/* Serie A Admin - ortak davranışlar
   - mobilde sidebar aç/kapat
   - data-confirm: silme gibi işlemlerden önce onay sor
   - maç formu: durum Canlı/Tamamlandı ise skor alanlarını göster
   - takım formu: renk seçiciler + arma önizleme */
(function () {
  'use strict';

  /* Sidebar (mobil) */
  const toggleBtn = document.querySelector('[data-sidebar-toggle]');
  const setSidebar = (open) => {
    document.body.classList.toggle('sidebar-open', open);
    if (toggleBtn) toggleBtn.setAttribute('aria-expanded', String(open));
  };
  if (toggleBtn) toggleBtn.addEventListener('click', () => setSidebar(!document.body.classList.contains('sidebar-open')));
  const backdrop = document.querySelector('[data-sidebar-close]');
  if (backdrop) backdrop.addEventListener('click', () => setSidebar(false));
  document.addEventListener('keydown', (e) => { if (e.key === 'Escape') setSidebar(false); });

  /* Onay isteyen link ve formlar */
  document.addEventListener('click', (e) => {
    const el = e.target.closest('a[data-confirm]');
    if (el && !confirm(el.dataset.confirm)) e.preventDefault();
  });
  document.querySelectorAll('form[data-confirm]').forEach((form) =>
    form.addEventListener('submit', (e) => { if (!confirm(form.dataset.confirm)) e.preventDefault(); }));

  /* Maç formu: skor alanları */
  const statusSelect = document.querySelector('[data-status-select]');
  const scoreFields = document.querySelector('[data-score-fields]');
  if (statusSelect && scoreFields) {
    const sync = () => { scoreFields.hidden = !['Live', 'Finished'].includes(statusSelect.value); };
    statusSelect.addEventListener('change', sync);
    sync();
  }

  /* Takım formu: renk kodları + arma önizleme */
  const crest = document.querySelector('[data-preview-crest]');
  const colorA = document.querySelector('[data-color-input="a"]');
  const colorB = document.querySelector('[data-color-input="b"]');
  const code = document.querySelector('[data-code-input]');
  const name = document.querySelector('[data-name-input]');
  const previewName = document.querySelector('[data-preview-name]');
  function updatePreview() {
    if (colorA) document.querySelector('[data-color-a]').textContent = colorA.value.toUpperCase();
    if (colorB) document.querySelector('[data-color-b]').textContent = colorB.value.toUpperCase();
    if (crest) {
      crest.style.setProperty('--crest-a', colorA ? colorA.value : '#1E5BD8');
      crest.style.setProperty('--crest-b', colorB ? colorB.value : '#FFFFFF');
      crest.textContent = (code && code.value.trim()) ? code.value.trim().toUpperCase() : '---';
    }
    if (previewName) previewName.textContent = (name && name.value.trim()) || 'Takım adı';
  }
  [colorA, colorB, code, name].forEach((el) => { if (el) el.addEventListener('input', updatePreview); });
  if (crest) updatePreview();
})();
