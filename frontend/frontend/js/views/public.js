// Home (course catalog) + course detail (chapters, reviews, enrollment, exams list).
(function () {
  const { el, escapeHtml, formatTimeSpan, stars, errorToast, toast, confirmAction } = App.ui;

  async function viewHome(container) {
    container.innerHTML = `
      <div class="page-header">
        <div>
          <h1>Kolegiji</h1>
          <div class="muted">Pregledaj dostupne kolegije i upiši se.</div>
        </div>
        <input id="courseSearch" type="text" placeholder="Pretraži po nazivu…" style="min-width:220px;" />
      </div>
      <div id="courseGrid" class="grid"><div class="empty-state">Učitavanje…</div></div>
    `;

    let courses = [];
    try {
      courses = (await App.api.courses.getAll()) || [];
    } catch (err) {
      container.querySelector("#courseGrid").innerHTML = `<div class="empty-state">Greška: ${escapeHtml(err.message)}</div>`;
      return;
    }

    function render(list) {
      const grid = container.querySelector("#courseGrid");
      if (!list.length) {
        grid.innerHTML = `<div class="empty-state">Nema kolegija za prikaz.</div>`;
        return;
      }
      grid.innerHTML = "";
      list.forEach((c) => {
        const card = el(`
          <div class="course-card">
            <h3>${escapeHtml(c.name)}</h3>
            <div class="meta">Trajanje: ${formatTimeSpan(c.timeToComplete)} · ${(c.chapters || []).length} poglavlja</div>
            <a href="#/course/${c.id}" class="btn btn-primary btn-sm" style="align-self:flex-start;">Pogledaj</a>
          </div>
        `);
        grid.appendChild(card);
      });
    }

    render(courses);

    container.querySelector("#courseSearch").addEventListener("input", (e) => {
      const q = e.target.value.trim().toLowerCase();
      render(courses.filter((c) => c.name.toLowerCase().includes(q)));
    });
  }

  async function viewCourseDetail(container, { id }) {
    container.innerHTML = `<div class="empty-state">Učitavanje…</div>`;
    let course;
    try {
      course = await App.api.courses.get(id);
    } catch (err) {
      container.innerHTML = `<div class="card empty-state">Kolegij nije pronađen. ${escapeHtml(err.message)}</div>`;
      return;
    }

    const user = App.auth.currentUser();

    container.innerHTML = `
      <div class="breadcrumbs"><a href="#/">Kolegiji</a> / ${escapeHtml(course.name)}</div>
      <div class="page-header">
        <div>
          <h1>${escapeHtml(course.name)}</h1>
          <div class="muted">Ukupno trajanje: ${formatTimeSpan(course.timeToComplete)}</div>
        </div>
        <div id="enrollArea"></div>
      </div>

      <div class="split two-col">
        <div class="section">
          <h2>Poglavlja</h2>
          <div id="chaptersArea"></div>
        </div>
        <div class="section">
          <h2>Ispiti</h2>
          <div id="examsArea"></div>
        </div>
      </div>

      <div class="section">
        <h2>Recenzije</h2>
        <div id="reviewsArea"></div>
      </div>
    `;

    // --- Enrollment ---
    const enrollArea = container.querySelector("#enrollArea");
    if (!user) {
      enrollArea.innerHTML = `<a href="#/login" class="btn btn-primary">Prijavi se za upis</a>`;
    } else {
      try {
        const enrollments = await App.api.enrollments.byUser(user.id);
        const existing = (enrollments || []).find((e) => e.courseId == course.id);
        if (existing) {
          if (existing.completed) {
            enrollArea.innerHTML = `<span class="pill pill-success">Završeno ✓</span>`;
          } else {
            enrollArea.innerHTML = `<button class="btn btn-primary" id="completeBtn">Označi kao završeno</button>`;
            enrollArea.querySelector("#completeBtn").addEventListener("click", async () => {
              try {
                await App.api.enrollments.complete(user.id, course.id);
                toast("Kolegij označen kao završen.", "success");
                App.router.resolve();
              } catch (err) {
                errorToast(err);
              }
            });
          }
        } else {
          enrollArea.innerHTML = `<button class="btn btn-primary" id="enrollBtn">Upiši se</button>`;
          enrollArea.querySelector("#enrollBtn").addEventListener("click", async () => {
            try {
              await App.api.enrollments.enroll({ courseId: Number(course.id), userId: user.id });
              toast("Upis uspješan.", "success");
              App.router.resolve();
            } catch (err) {
              errorToast(err);
            }
          });
        }
      } catch (err) {
        enrollArea.innerHTML = `<span class="muted small">Ne mogu provjeriti upis: ${escapeHtml(err.message)}</span>`;
      }
    }

    // --- Chapters (requires login per API) ---
    const chaptersArea = container.querySelector("#chaptersArea");
    if (!user) {
      chaptersArea.innerHTML = `<div class="muted small">Prijavi se za pregled poglavlja.</div>`;
    } else {
      try {
        const chapters = await App.api.chapters.byCourse(course.id);
        chaptersArea.innerHTML = (chapters || []).length
          ? `<ol>${chapters.map((c) => `<li>${escapeHtml(c.name)} <span class="muted small">(${formatTimeSpan(c.length)})</span></li>`).join("")}</ol>`
          : `<div class="muted small">Nema poglavlja.</div>`;
      } catch (err) {
        chaptersArea.innerHTML = `<div class="muted small">Greška: ${escapeHtml(err.message)}</div>`;
      }
    }

    // --- Exams (requires login per API) ---
    const examsArea = container.querySelector("#examsArea");
    if (!user) {
      examsArea.innerHTML = `<div class="muted small">Prijavi se za pregled ispita.</div>`;
    } else {
      try {
        const exams = await App.api.exams.byCourse(course.id);
        examsArea.innerHTML = (exams || []).length
          ? exams
              .map(
                (e) => `
              <div class="card" style="margin-bottom:10px;display:flex;justify-content:space-between;align-items:center;">
                <div>
                  <strong>${escapeHtml(e.title)}</strong>
                  <div class="muted small">Vrijeme: ${formatTimeSpan(e.allotedTime)}</div>
                </div>
                <a href="#/exam/${e.id}" class="btn btn-primary btn-sm">Polaži</a>
              </div>`
              )
              .join("")
          : `<div class="muted small">Nema ispita za ovaj kolegij.</div>`;
      } catch (err) {
        examsArea.innerHTML = `<div class="muted small">Greška: ${escapeHtml(err.message)}</div>`;
      }
    }

    // --- Reviews (requires login per API) ---
    const reviewsArea = container.querySelector("#reviewsArea");
    if (!user) {
      reviewsArea.innerHTML = `<div class="muted small">Prijavi se za pregled i pisanje recenzija.</div>`;
      return;
    }

    async function loadReviews() {
      let reviews = [];
      try {
        reviews = (await App.api.reviews.byCourse(course.id)) || [];
      } catch (err) {
        reviewsArea.innerHTML = `<div class="muted small">Greška: ${escapeHtml(err.message)}</div>`;
        return;
      }

      const myReview = reviews.find((r) => r.userId === user.id);

      reviewsArea.innerHTML = `
        <div id="reviewsList">
          ${
            reviews.length
              ? reviews
                  .map(
                    (r) => `
                <div class="card" style="margin-bottom:8px;">
                  <div class="stars">${stars(r.numOfStars)}</div>
                  <div>${escapeHtml(r.text)}</div>
                  <div class="muted small">${escapeHtml(r.dateOfReview)}${r.userId === user.id ? " · tvoja recenzija" : ""}</div>
                  ${
                    r.userId === user.id
                      ? `<div style="margin-top:6px;display:flex;gap:8px;">
                          <button class="btn btn-sm" data-edit-review="${r.id}">Uredi</button>
                          <button class="btn btn-sm btn-danger" data-delete-review="${r.id}">Obriši</button>
                        </div>`
                      : App.auth.isAdmin()
                      ? `<div style="margin-top:6px;"><button class="btn btn-sm btn-danger" data-delete-review="${r.id}">Obriši</button></div>`
                      : ""
                  }
                </div>`
                  )
                  .join("")
              : `<div class="muted small">Još nema recenzija.</div>`
          }
        </div>
        <div id="reviewFormHolder" style="margin-top:14px;"></div>
      `;

      reviewsArea.querySelectorAll("[data-delete-review]").forEach((btn) => {
        btn.addEventListener("click", async () => {
          if (!confirmAction("Obrisati recenziju?")) return;
          try {
            await App.api.reviews.delete(btn.dataset.deleteReview);
            toast("Recenzija obrisana.", "success");
            loadReviews();
          } catch (err) {
            errorToast(err);
          }
        });
      });

      const formHolder = reviewsArea.querySelector("#reviewFormHolder");
      if (!myReview) {
        formHolder.innerHTML = `
          <form class="form" id="newReviewForm">
            <h3>Ostavi recenziju</h3>
            <div class="form-row">
              <label>Ocjena (1-5)</label>
              <input type="number" name="numOfStars" min="1" max="5" value="5" required />
            </div>
            <div class="form-row">
              <label>Komentar</label>
              <textarea name="text" rows="3" required></textarea>
            </div>
            <button class="btn btn-primary" type="submit">Objavi</button>
          </form>
        `;
        formHolder.querySelector("#newReviewForm").addEventListener("submit", async (e) => {
          e.preventDefault();
          const fd = new FormData(e.target);
          try {
            await App.api.reviews.create({
              text: fd.get("text"),
              numOfStars: Number(fd.get("numOfStars")),
              courseId: Number(course.id),
            });
            toast("Recenzija objavljena.", "success");
            loadReviews();
          } catch (err) {
            errorToast(err);
          }
        });
      } else {
        reviewsArea.querySelectorAll("[data-edit-review]").forEach((btn) => {
          btn.addEventListener("click", () => {
            formHolder.innerHTML = `
              <form class="form" id="editReviewForm">
                <h3>Uredi recenziju</h3>
                <div class="form-row">
                  <label>Ocjena (1-5)</label>
                  <input type="number" name="numOfStars" min="1" max="5" value="${myReview.numOfStars}" required />
                </div>
                <div class="form-row">
                  <label>Komentar</label>
                  <textarea name="text" rows="3" required>${escapeHtml(myReview.text)}</textarea>
                </div>
                <button class="btn btn-primary" type="submit">Spremi</button>
              </form>
            `;
            formHolder.querySelector("#editReviewForm").addEventListener("submit", async (e) => {
              e.preventDefault();
              const fd = new FormData(e.target);
              try {
                await App.api.reviews.update(myReview.id, {
                  text: fd.get("text"),
                  numOfStars: Number(fd.get("numOfStars")),
                });
                toast("Recenzija ažurirana.", "success");
                loadReviews();
              } catch (err) {
                errorToast(err);
              }
            });
          });
        });
      }
    }

    await loadReviews();
  }

  window.App = window.App || {};
  window.App.views = window.App.views || {};
  window.App.views.home = viewHome;
  window.App.views.courseDetail = viewCourseDetail;
})();
