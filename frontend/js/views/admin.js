// Admin / Instructor content-management panel.
// Organized as tabs; visibility of create/update/delete controls follows the API's own
// [Authorize(Roles=...)] rules on each endpoint (see Presentation/Controllers/*.cs):
//   Course:            read public, write Admin only
//   Chapter:           read Authorize, write Admin only
//   Exam:              read Authorize, write Admin+Instructor
//   Question:          read Authorize, write Admin+Instructor, delete Admin only
//   Answer:            no "list by question" endpoint -> answers are read from the
//                      QuestionDto.Answers collection; write Admin+Instructor, delete Admin only
//   Role/UserRole/User management: Admin only
(function () {
  const { escapeHtml, formatTimeSpan, timeSpanString, parseTimeSpan, errorToast, toast, confirmAction, renderCrudTable, el } = App.ui;

  const TABS = [
    { key: "courses", label: "Kolegiji i poglavlja", roles: ["Admin", "Instructor"] },
    { key: "exams", label: "Ispiti, pitanja i odgovori", roles: ["Admin", "Instructor"] },
    { key: "users", label: "Korisnici i uloge", roles: ["Admin"] },
    { key: "reports", label: "Izvještaji", roles: ["Admin", "Instructor"] },
  ];

  async function viewAdmin(container) {
    const visibleTabs = TABS.filter((t) => App.auth.hasRole(...t.roles));
    let active = visibleTabs[0]?.key;

    container.innerHTML = `
      <h1>Admin panel</h1>
      <div class="tabs" id="adminTabs"></div>
      <div id="adminBody"></div>
    `;

    const tabsEl = container.querySelector("#adminTabs");
    const bodyEl = container.querySelector("#adminBody");

    function renderTabs() {
      tabsEl.innerHTML = "";
      visibleTabs.forEach((t) => {
        const btn = el(`<button class="tab-btn${t.key === active ? " active" : ""}">${escapeHtml(t.label)}</button>`);
        btn.addEventListener("click", () => {
          active = t.key;
          renderTabs();
          renderBody();
        });
        tabsEl.appendChild(btn);
      });
    }

    function renderBody() {
      bodyEl.innerHTML = "";
      if (active === "courses") renderCoursesTab(bodyEl);
      else if (active === "exams") renderExamsTab(bodyEl);
      else if (active === "users") renderUsersTab(bodyEl);
      else if (active === "reports") renderReportsTab(bodyEl);
    }

    renderTabs();
    renderBody();
  }

  // ---------- Courses + Chapters ----------
  function renderCoursesTab(container) {
    const isAdmin = App.auth.isAdmin();
    const chapterHolder = el(`<div class="section" id="chapterHolder"></div>`);

    renderCrudTable(container, {
      title: "Kolegiji",
      columns: [
        { key: "id", label: "ID" },
        { key: "name", label: "Naziv" },
        { key: "timeToComplete", label: "Trajanje", render: (r) => formatTimeSpan(r.timeToComplete) },
      ],
      getId: (r) => r.id,
      fetchList: App.api.courses.getAll,
      formFields: [
        { name: "name", label: "Naziv", required: true },
        { name: "hours", label: "Sati", type: "number", min: 0, default: 0 },
        { name: "minutes", label: "Minute", type: "number", min: 0, default: 0 },
      ],
      buildCreatePayload: (raw) => ({ name: raw.name, timeToComplete: timeSpanString(raw.hours, raw.minutes) }),
      buildUpdatePayload: (raw) => ({ name: raw.name, timeToComplete: timeSpanString(raw.hours, raw.minutes) }),
      onCreate: isAdmin ? App.api.courses.create : null,
      onUpdate: isAdmin ? App.api.courses.update : null,
      onDelete: isAdmin ? App.api.courses.delete : null,
      extraRowActions: isAdmin
        ? [{ label: "Poglavlja", onClick: (row) => renderChapters(chapterHolder, row) }]
        : [],
    });

    container.appendChild(chapterHolder);
  }

  function renderChapters(holder, course) {
    holder.innerHTML = "";
    const wrap = el(`<div></div>`);
    wrap.innerHTML = `<h3 style="margin-top:6px;">Poglavlja — ${escapeHtml(course.name)}</h3>`;
    holder.appendChild(wrap);

    renderCrudTable(wrap, {
      title: "",
      fetchList: () => App.api.chapters.byCourse(course.id),
      columns: [
        { key: "id", label: "ID" },
        { key: "name", label: "Naziv" },
        { key: "length", label: "Trajanje", render: (r) => formatTimeSpan(r.length) },
      ],
      getId: (r) => r.id,
      formFields: [
        { name: "name", label: "Naziv", required: true },
        { name: "hours", label: "Sati", type: "number", min: 0, default: 0 },
        { name: "minutes", label: "Minute", type: "number", min: 0, default: 0 },
      ],
      buildCreatePayload: (raw) => ({ name: raw.name, length: timeSpanString(raw.hours, raw.minutes) }),
      buildUpdatePayload: (raw) => ({ name: raw.name, length: timeSpanString(raw.hours, raw.minutes) }),
      onCreate: (payload) => App.api.chapters.create(course.id, payload),
      onUpdate: App.api.chapters.update,
      onDelete: App.api.chapters.delete,
    });
  }

  // ---------- Exams -> Questions -> Answers drill-down ----------
  async function renderExamsTab(container) {
    const isAdmin = App.auth.isAdmin();
    let courses = [];
    try {
      courses = (await App.api.courses.getAll()) || [];
    } catch (err) {
      container.innerHTML = `<div class="empty-state">Greška pri dohvatu kolegija: ${escapeHtml(err.message)}</div>`;
      return;
    }

    const picker = el(`
      <div class="inline-select">
        <div class="form-row">
          <label>Kolegij</label>
          <select id="courseSelect">
            <option value="">— odaberi kolegij —</option>
            ${courses.map((c) => `<option value="${c.id}">${escapeHtml(c.name)} (#${c.id})</option>`).join("")}
          </select>
        </div>
      </div>
    `);
    container.appendChild(picker);

    const examHolder = el(`<div id="examHolder"></div>`);
    container.appendChild(examHolder);

    picker.querySelector("#courseSelect").addEventListener("change", (e) => {
      examHolder.innerHTML = "";
      const courseId = e.target.value;
      if (!courseId) return;
      renderExamsForCourse(examHolder, Number(courseId), isAdmin);
    });
  }

  function renderExamsForCourse(holder, courseId, isAdmin) {
    const questionHolder = el(`<div class="section" id="questionHolder"></div>`);

    renderCrudTable(holder, {
      title: "Ispiti",
      fetchList: () => App.api.exams.byCourse(courseId),
      columns: [
        { key: "id", label: "ID" },
        { key: "title", label: "Naslov" },
        { key: "allotedTime", label: "Vrijeme", render: (r) => formatTimeSpan(r.allotedTime) },
      ],
      getId: (r) => r.id,
      formFields: [
        { name: "title", label: "Naslov", required: true },
        { name: "hours", label: "Sati", type: "number", min: 0, default: 0 },
        { name: "minutes", label: "Minute", type: "number", min: 0, default: 30 },
      ],
      buildCreatePayload: (raw) => ({ title: raw.title, allotedTime: timeSpanString(raw.hours, raw.minutes), courseId }),
      buildUpdatePayload: (raw) => ({ title: raw.title, allotedTime: timeSpanString(raw.hours, raw.minutes), courseId }),
      onCreate: App.api.exams.create,
      onUpdate: App.api.exams.update,
      onDelete: App.api.exams.delete,
      extraRowActions: [{ label: "Pitanja", onClick: (row) => renderQuestions(questionHolder, row, isAdmin) }],
    });

    holder.appendChild(questionHolder);
  }

  function renderQuestions(holder, exam, isAdmin) {
    holder.innerHTML = "";
    const wrap = el(`<div></div>`);
    wrap.innerHTML = `<h3 style="margin-top:6px;">Pitanja — ${escapeHtml(exam.title)}</h3>`;
    holder.appendChild(wrap);

    const answerHolder = el(`<div class="section" id="answerHolder"></div>`);

    renderCrudTable(wrap, {
      title: "",
      fetchList: () => App.api.questions.byExam(exam.id),
      columns: [
        { key: "id", label: "ID" },
        { key: "text", label: "Tekst" },
        { key: "type", label: "Tip", render: (r) => (Number(r.type) === 1 ? "Točno/Netočno" : "Višestruki izbor") },
        { key: "answers", label: "Odgovori", render: (r) => `${(r.answers || []).length}` },
      ],
      getId: (r) => r.id,
      formFields: [
        { name: "text", label: "Tekst pitanja", type: "textarea", required: true },
        {
          name: "type",
          label: "Tip",
          type: "select",
          options: [
            { value: "0", label: "Višestruki izbor" },
            { value: "1", label: "Točno/Netočno" },
          ],
        },
      ],
      buildCreatePayload: (raw) => ({ text: raw.text, type: Number(raw.type), examId: exam.id }),
      buildUpdatePayload: (raw) => ({ text: raw.text, type: Number(raw.type), examId: exam.id }),
      onCreate: App.api.questions.create,
      onUpdate: App.api.questions.update,
      onDelete: isAdmin ? App.api.questions.delete : null,
      canDelete: isAdmin,
      extraRowActions: [{ label: "Odgovori", onClick: (row) => renderAnswers(answerHolder, row, isAdmin) }],
    });

    wrap.appendChild(answerHolder);
  }

  function renderAnswers(holder, question, isAdmin) {
    holder.innerHTML = "";
    const wrap = el(`<div></div>`);
    wrap.innerHTML = `
      <h3 style="margin-top:6px;">Odgovori — ${escapeHtml(question.text)}</h3>
      <p class="muted small">Napomena: API-jev odgovor (AnswerDto) ne vraća je li odgovor točan, pa se prilikom uređivanja polje "Točan" ne može unaprijed popuniti — potvrdi ga eksplicitno da postaviš odgovor kao točan.</p>
    `;
    holder.appendChild(wrap);

    renderCrudTable(wrap, {
      title: "",
      fetchList: async () => {
        const fresh = await App.api.questions.get(question.id);
        return fresh.answers || [];
      },
      columns: [
        { key: "id", label: "ID" },
        { key: "option", label: "Tekst odgovora" },
      ],
      getId: (r) => r.id,
      formFields: [
        { name: "option", label: "Tekst odgovora", required: true },
        { name: "correct", label: "Točan odgovor", type: "checkbox", default: false },
      ],
      buildCreatePayload: (raw) => ({ questionId: question.id, option: raw.option, correct: !!raw.correct }),
      buildUpdatePayload: (raw) => ({ questionId: question.id, option: raw.option, correct: !!raw.correct }),
      onCreate: App.api.answers.create,
      onUpdate: App.api.answers.update,
      onDelete: isAdmin ? App.api.answers.delete : null,
      canDelete: isAdmin,
    });
  }

  // ---------- Users & Roles ----------
  async function renderUsersTab(container) {
    const roleHolder = el(`<div class="section" id="userRolesHolder"></div>`);

    let roles = [];
    try {
      roles = (await App.api.roles.getAll()) || [];
    } catch (err) {
      /* handled below via table error state */
    }

    renderCrudTable(container, {
      title: "Korisnici",
      fetchList: App.api.users.getAll,
      columns: [
        { key: "id", label: "ID" },
        { key: "name", label: "Ime" },
        { key: "surname", label: "Prezime" },
        { key: "email", label: "Email" },
      ],
      getId: (r) => r.id,
      formFields: [],
      onDelete: App.api.users.delete,
      extraRowActions: [{ label: "Uloge", onClick: (row) => renderUserRoles(roleHolder, row, roles) }],
      canCreate: false,
    });

    container.appendChild(roleHolder);

    const rolesSection = el(`<div class="section"></div>`);
    container.appendChild(rolesSection);
    renderCrudTable(rolesSection, {
      title: "Uloge (Roles)",
      description: "Sustavne uloge korištene za autorizaciju (Admin / User / Instructor).",
      fetchList: App.api.roles.getAll,
      columns: [
        { key: "id", label: "ID" },
        { key: "name", label: "Naziv" },
      ],
      getId: (r) => r.id,
      formFields: [{ name: "name", label: "Naziv", required: true }],
      onCreate: App.api.roles.create,
      onDelete: App.api.roles.delete,
    });
  }

  async function renderUserRoles(holder, user, roles) {
    holder.innerHTML = "";
    const wrap = el(`<div></div>`);
    wrap.innerHTML = `<h3 style="margin-top:6px;">Uloge — ${escapeHtml(user.name)} ${escapeHtml(user.surname)} (${escapeHtml(user.email)})</h3>
      <div id="currentRoles" class="muted small">Učitavanje…</div>
      <div class="inline-select" style="margin-top:10px;">
        <div class="form-row">
          <label>Dodaj ulogu</label>
          <select id="roleSelect">${roles.map((r) => `<option value="${r.id}">${escapeHtml(r.name)}</option>`).join("")}</select>
        </div>
        <button class="btn btn-primary btn-sm" id="assignRoleBtn">Dodaj</button>
      </div>
    `;
    holder.appendChild(wrap);

    async function loadCurrent() {
      const box = wrap.querySelector("#currentRoles");
      try {
        const userRoles = await App.api.userRoles.byUser(user.id);
        box.innerHTML = (userRoles || []).length
          ? userRoles
              .map((ur) => {
                const role = roles.find((r) => r.id === ur.roleId);
                return `<span class="pill pill-muted" style="margin-right:6px;">${escapeHtml(role ? role.name : "#" + ur.roleId)}
                  <button class="link-btn" data-remove-role="${ur.roleId}" style="margin-left:6px;color:var(--danger);">✕</button></span>`;
              })
              .join("")
          : "Nema dodijeljenih uloga.";
        box.querySelectorAll("[data-remove-role]").forEach((btn) => {
          btn.addEventListener("click", async () => {
            if (!confirmAction("Ukloniti ovu ulogu korisniku?")) return;
            try {
              await App.api.userRoles.remove(user.id, btn.dataset.removeRole);
              toast("Uloga uklonjena.", "success");
              loadCurrent();
            } catch (err) {
              errorToast(err);
            }
          });
        });
      } catch (err) {
        box.innerHTML = `Greška: ${escapeHtml(err.message)}`;
      }
    }

    wrap.querySelector("#assignRoleBtn").addEventListener("click", async () => {
      const roleId = Number(wrap.querySelector("#roleSelect").value);
      try {
        await App.api.userRoles.assign({ userId: user.id, roleId });
        toast("Uloga dodijeljena.", "success");
        loadCurrent();
      } catch (err) {
        errorToast(err);
      }
    });

    loadCurrent();
  }

  // ---------- Reports ----------
  function renderReportsTab(container) {
    const isAdmin = App.auth.isAdmin();

    const attemptsSection = el(`<div class="section"></div>`);
    attemptsSection.innerHTML = `
      <h3>Pokušaji ispita</h3>
      <div class="inline-select">
        <div class="form-row"><label>ID ispita</label><input id="examIdInput" type="number" min="1" /></div>
        <button class="btn btn-sm" id="loadByExamBtn">Prikaži po ispitu</button>
        ${isAdmin ? `<button class="btn btn-sm" id="loadAllAttemptsBtn">Prikaži sve</button>` : ""}
      </div>
      <div id="attemptsResult"></div>
    `;
    container.appendChild(attemptsSection);

    function renderAttempts(list) {
      const box = attemptsSection.querySelector("#attemptsResult");
      if (!list || !list.length) {
        box.innerHTML = `<div class="muted small">Nema podataka.</div>`;
        return;
      }
      box.innerHTML = `
        <div class="table-wrap"><table>
          <thead><tr><th>ID</th><th>Ispit</th><th>Korisnik</th><th>Bodovi</th><th>Rezultat</th></tr></thead>
          <tbody>
            ${list
              .map(
                (a) => `<tr>
                  <td>${a.id}</td><td>${a.examId}</td><td>${a.userId}</td><td>${Number(a.score).toFixed(1)}</td>
                  <td>${a.passed ? '<span class="pill pill-success">Položeno</span>' : '<span class="pill pill-danger">Nije</span>'}</td>
                </tr>`
              )
              .join("")}
          </tbody>
        </table></div>
      `;
    }

    attemptsSection.querySelector("#loadByExamBtn").addEventListener("click", async () => {
      const examId = attemptsSection.querySelector("#examIdInput").value;
      if (!examId) return toast("Unesi ID ispita.", "error");
      try {
        renderAttempts(await App.api.examAttempts.byExam(examId));
      } catch (err) {
        errorToast(err);
      }
    });
    if (isAdmin) {
      attemptsSection.querySelector("#loadAllAttemptsBtn").addEventListener("click", async () => {
        try {
          renderAttempts(await App.api.examAttempts.getAll());
        } catch (err) {
          errorToast(err);
        }
      });
    }

    const reviewsSection = el(`<div class="section"></div>`);
    reviewsSection.innerHTML = `
      <h3>Recenzije</h3>
      <div class="inline-select">
        <div class="form-row"><label>ID kolegija</label><input id="courseIdInput" type="number" min="1" /></div>
        <button class="btn btn-sm" id="loadByCourseBtn">Prikaži po kolegiju</button>
        ${isAdmin ? `<button class="btn btn-sm" id="loadAllReviewsBtn">Prikaži sve</button>` : ""}
      </div>
      <div id="reviewsResult"></div>
    `;
    container.appendChild(reviewsSection);

    function renderReviewsTable(list) {
      const box = reviewsSection.querySelector("#reviewsResult");
      if (!list || !list.length) {
        box.innerHTML = `<div class="muted small">Nema podataka.</div>`;
        return;
      }
      box.innerHTML = `
        <div class="table-wrap"><table>
          <thead><tr><th>ID</th><th>Kolegij</th><th>Korisnik</th><th>Ocjena</th><th>Tekst</th><th>Datum</th></tr></thead>
          <tbody>
            ${list
              .map(
                (r) =>
                  `<tr><td>${r.id}</td><td>${r.courseId}</td><td>${r.userId}</td><td>${r.numOfStars}</td><td>${escapeHtml(
                    r.text
                  )}</td><td>${escapeHtml(r.dateOfReview)}</td></tr>`
              )
              .join("")}
          </tbody>
        </table></div>
      `;
    }

    reviewsSection.querySelector("#loadByCourseBtn").addEventListener("click", async () => {
      const courseId = reviewsSection.querySelector("#courseIdInput").value;
      if (!courseId) return toast("Unesi ID kolegija.", "error");
      try {
        renderReviewsTable(await App.api.reviews.byCourse(courseId));
      } catch (err) {
        errorToast(err);
      }
    });
    if (isAdmin) {
      reviewsSection.querySelector("#loadAllReviewsBtn").addEventListener("click", async () => {
        try {
          renderReviewsTable(await App.api.reviews.getAll());
        } catch (err) {
          errorToast(err);
        }
      });
    }
  }

  window.App = window.App || {};
  window.App.views = window.App.views || {};
  window.App.views.admin = viewAdmin;
})();
