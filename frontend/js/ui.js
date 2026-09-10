// Small shared UI helpers: toasts, escaping, formatting, DOM shorthands, generic CRUD table.
(function () {
  function toast(message, type = "info") {
    const container = document.getElementById("toastContainer");
    const el = document.createElement("div");
    el.className = "toast" + (type !== "info" ? " " + type : "");
    el.textContent = message;
    container.appendChild(el);
    setTimeout(() => el.remove(), 4000);
  }

  function errorToast(err) {
    toast(err && err.message ? err.message : "Došlo je do greške.", "error");
  }

  function escapeHtml(str) {
    if (str === null || str === undefined) return "";
    return String(str)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#39;");
  }

  // TimeSpan comes back from the API as "hh:mm:ss" (System.Text.Json TimeSpan format).
  function parseTimeSpan(value) {
    if (!value) return { hours: 0, minutes: 0 };
    const match = /^(\d+)\.(\d{2}):(\d{2}):(\d{2})|(\d{2}):(\d{2}):(\d{2})/.exec(value);
    if (!match) return { hours: 0, minutes: 0 };
    if (match[1] !== undefined) {
      return { hours: parseInt(match[1], 10) * 24 + parseInt(match[2], 10), minutes: parseInt(match[3], 10) };
    }
    return { hours: parseInt(match[5], 10), minutes: parseInt(match[6], 10) };
  }

  function formatTimeSpan(value) {
    const { hours, minutes } = parseTimeSpan(value);
    if (!hours && !minutes) return "0 min";
    const parts = [];
    if (hours) parts.push(hours + "h");
    if (minutes) parts.push(minutes + "min");
    return parts.join(" ");
  }

  function timeSpanString(hours, minutes) {
    const h = String(Math.max(0, parseInt(hours, 10) || 0)).padStart(2, "0");
    const m = String(Math.max(0, parseInt(minutes, 10) || 0)).padStart(2, "0");
    return `${h}:${m}:00`;
  }

  function formatDate(dateOnly) {
    if (!dateOnly) return "";
    return dateOnly;
  }

  function stars(n) {
    const count = Math.max(0, Math.min(5, Number(n) || 0));
    return "★".repeat(count) + "☆".repeat(5 - count);
  }

  function el(html) {
    const template = document.createElement("template");
    template.innerHTML = html.trim();
    return template.content.firstElementChild;
  }

  function confirmAction(message) {
    return window.confirm(message);
  }

  // Generic CRUD table used across the admin/instructor panel to avoid re-implementing
  // list + create + inline edit + delete for every single resource.
  function renderCrudTable(container, opts) {
    const {
      title,
      description,
      fetchList,
      columns,
      getId,
      formFields,
      buildCreatePayload,
      buildUpdatePayload,
      onCreate,
      onUpdate,
      onDelete,
      extraRowActions,
      canCreate = true,
      canDelete = true,
    } = opts;

    const wrap = el(`<div class="section"></div>`);
    wrap.innerHTML = `
      <div class="section-title">
        <div>
          <h3>${escapeHtml(title)}</h3>
          ${description ? `<div class="muted small">${escapeHtml(description)}</div>` : ""}
        </div>
        ${canCreate && onCreate ? `<button class="btn btn-primary btn-sm" data-action="toggle-create">+ Novo</button>` : ""}
      </div>
      <div class="create-form-holder"></div>
      <div class="table-wrap"><table><thead></thead><tbody></tbody></table></div>
    `;
    container.appendChild(wrap);

    const thead = wrap.querySelector("thead");
    const tbody = wrap.querySelector("tbody");
    const createHolder = wrap.querySelector(".create-form-holder");

    const headRow = document.createElement("tr");
    columns.forEach((c) => headRow.appendChild(el(`<th>${escapeHtml(c.label)}</th>`)));
    if (onUpdate || onDelete || (extraRowActions && extraRowActions.length)) {
      headRow.appendChild(el(`<th>Akcije</th>`));
    }
    thead.appendChild(headRow);

    function buildFieldsHtml(fields, values = {}) {
      return fields
        .map((f) => {
          const val = values[f.name] !== undefined ? values[f.name] : f.default !== undefined ? f.default : "";
          if (f.type === "select") {
            const opts = (f.options || [])
              .map(
                (o) =>
                  `<option value="${escapeHtml(o.value)}" ${String(o.value) === String(val) ? "selected" : ""}>${escapeHtml(
                    o.label
                  )}</option>`
              )
              .join("");
            return `<div class="form-row"><label>${escapeHtml(f.label)}</label><select name="${f.name}" ${
              f.required ? "required" : ""
            }>${opts}</select></div>`;
          }
          if (f.type === "checkbox") {
            return `<div class="checkbox-row"><input type="checkbox" name="${f.name}" ${val ? "checked" : ""} /><label>${escapeHtml(
              f.label
            )}</label></div>`;
          }
          if (f.type === "textarea") {
            return `<div class="form-row"><label>${escapeHtml(f.label)}</label><textarea name="${f.name}" ${
              f.required ? "required" : ""
            } rows="3">${escapeHtml(val)}</textarea></div>`;
          }
          return `<div class="form-row"><label>${escapeHtml(f.label)}</label><input name="${f.name}" type="${
            f.type || "text"
          }" value="${escapeHtml(val)}" ${f.required ? "required" : ""} ${f.min !== undefined ? `min="${f.min}"` : ""} /></div>`;
        })
        .join("");
    }

    function readForm(formEl, fields) {
      const data = {};
      fields.forEach((f) => {
        const input = formEl.elements[f.name];
        if (!input) return;
        if (f.type === "checkbox") data[f.name] = input.checked;
        else if (f.type === "number") data[f.name] = input.value === "" ? null : Number(input.value);
        else data[f.name] = input.value;
      });
      return data;
    }

    function closeCreateForm() {
      createHolder.innerHTML = "";
    }

    if (canCreate && onCreate) {
      wrap.querySelector('[data-action="toggle-create"]').addEventListener("click", () => {
        if (createHolder.children.length) {
          closeCreateForm();
          return;
        }
        const form = el(`
          <form class="form card" style="margin-bottom:14px;max-width:520px;">
            ${buildFieldsHtml(formFields)}
            <div style="display:flex;gap:8px;">
              <button type="submit" class="btn btn-primary btn-sm">Spremi</button>
              <button type="button" class="btn btn-sm" data-action="cancel">Odustani</button>
            </div>
          </form>
        `);
        form.querySelector('[data-action="cancel"]').addEventListener("click", closeCreateForm);
        form.addEventListener("submit", async (e) => {
          e.preventDefault();
          const raw = readForm(form, formFields);
          const payload = buildCreatePayload ? buildCreatePayload(raw) : raw;
          try {
            await onCreate(payload);
            toast(`${title}: kreirano.`, "success");
            closeCreateForm();
            await refresh();
          } catch (err) {
            errorToast(err);
          }
        });
        createHolder.appendChild(form);
      });
    }

    async function refresh() {
      tbody.innerHTML = `<tr><td colspan="${columns.length + 1}" class="muted">Učitavanje…</td></tr>`;
      let rows;
      try {
        rows = (await fetchList()) || [];
      } catch (err) {
        tbody.innerHTML = `<tr><td colspan="${columns.length + 1}" class="muted">Greška: ${escapeHtml(err.message)}</td></tr>`;
        return;
      }
      if (!rows.length) {
        tbody.innerHTML = `<tr><td colspan="${columns.length + 1}" class="muted">Nema podataka.</td></tr>`;
        return;
      }
      tbody.innerHTML = "";
      rows.forEach((row) => {
        const tr = document.createElement("tr");
        columns.forEach((c) => {
          const td = document.createElement("td");
          td.innerHTML = c.render ? c.render(row) : escapeHtml(row[c.key]);
          tr.appendChild(td);
        });

        if (onUpdate || onDelete || (extraRowActions && extraRowActions.length)) {
          const actionsTd = document.createElement("td");
          actionsTd.className = "actions-cell";

          if (extraRowActions) {
            extraRowActions.forEach((a) => {
              const btn = el(`<button class="btn btn-sm">${escapeHtml(a.label)}</button>`);
              btn.addEventListener("click", () => a.onClick(row));
              actionsTd.appendChild(btn);
            });
          }

          if (onUpdate) {
            const editBtn = el(`<button class="btn btn-sm">Uredi</button>`);
            editBtn.addEventListener("click", () => {
              if (tr.classList.contains("editing")) return;
              tr.classList.add("editing");
              const editForm = el(`<form class="form" style="gap:8px;">${buildFieldsHtml(formFields, row)}</form>`);
              const saveRow = document.createElement("td");
              saveRow.colSpan = columns.length + 1;
              saveRow.appendChild(editForm);
              const actionsDiv = document.createElement("div");
              actionsDiv.style.display = "flex";
              actionsDiv.style.gap = "8px";
              const saveBtn = el(`<button type="button" class="btn btn-primary btn-sm">Spremi</button>`);
              const cancelBtn = el(`<button type="button" class="btn btn-sm">Odustani</button>`);
              actionsDiv.appendChild(saveBtn);
              actionsDiv.appendChild(cancelBtn);
              editForm.appendChild(actionsDiv);

              const editTr = document.createElement("tr");
              editTr.appendChild(saveRow);
              tr.after(editTr);

              cancelBtn.addEventListener("click", () => {
                editTr.remove();
                tr.classList.remove("editing");
              });
              saveBtn.addEventListener("click", async () => {
                const raw = readForm(editForm, formFields);
                const payload = buildUpdatePayload ? buildUpdatePayload(raw, row) : buildCreatePayload ? buildCreatePayload(raw) : raw;
                try {
                  await onUpdate(getId(row), payload);
                  toast(`${title}: ažurirano.`, "success");
                  await refresh();
                } catch (err) {
                  errorToast(err);
                }
              });
            });
            actionsTd.appendChild(editBtn);
          }

          if (onDelete && canDelete) {
            const delBtn = el(`<button class="btn btn-sm btn-danger">Obriši</button>`);
            delBtn.addEventListener("click", async () => {
              if (!confirmAction(`Obrisati ovaj zapis (${title})?`)) return;
              try {
                await onDelete(getId(row));
                toast(`${title}: obrisano.`, "success");
                await refresh();
              } catch (err) {
                errorToast(err);
              }
            });
            actionsTd.appendChild(delBtn);
          }

          tr.appendChild(actionsTd);
        }

        tbody.appendChild(tr);
      });
    }

    refresh();
    return { refresh, container: wrap };
  }

  window.App = window.App || {};
  window.App.ui = {
    toast,
    errorToast,
    escapeHtml,
    parseTimeSpan,
    formatTimeSpan,
    timeSpanString,
    formatDate,
    stars,
    el,
    confirmAction,
    renderCrudTable,
  };
})();
