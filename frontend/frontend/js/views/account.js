// Login, register, and profile (own account + enrolled courses) views.
(function () {
  const { escapeHtml, formatTimeSpan, errorToast, toast, confirmAction } = App.ui;

  function viewLogin(container) {
    container.innerHTML = `
      <div class="card" style="max-width:420px;margin:0 auto;">
        <h1>Prijava</h1>
        <form class="form" id="loginForm">
          <div class="form-row">
            <label>Email</label>
            <input type="email" name="email" required autocomplete="username" />
          </div>
          <div class="form-row">
            <label>Lozinka</label>
            <input type="password" name="password" required autocomplete="current-password" />
          </div>
          <button class="btn btn-primary" type="submit">Prijavi se</button>
        </form>
        <p class="small muted" style="margin-top:14px;">
          Nemaš račun? <a href="#/register">Registriraj se</a>
        </p>
        <p class="small muted">Zadani admin račun iz sjemena baze: <code class="kbd">admin@mail.com</code> / <code class="kbd">admin123</code></p>
      </div>
    `;

    container.querySelector("#loginForm").addEventListener("submit", async (e) => {
      e.preventDefault();
      const fd = new FormData(e.target);
      try {
        const token = await App.api.auth.login({ email: fd.get("email"), password: fd.get("password") });
        App.auth.setToken(token);
        toast("Uspješna prijava.", "success");
        App.router.navigate("/");
        App.main.renderNav();
      } catch (err) {
        errorToast(err);
      }
    });
  }

  function viewRegister(container) {
    container.innerHTML = `
      <div class="card" style="max-width:420px;margin:0 auto;">
        <h1>Registracija</h1>
        <form class="form" id="registerForm">
          <div class="form-row-inline">
            <div class="form-row">
              <label>Ime</label>
              <input type="text" name="name" required />
            </div>
            <div class="form-row">
              <label>Prezime</label>
              <input type="text" name="surname" required />
            </div>
          </div>
          <div class="form-row">
            <label>Email</label>
            <input type="email" name="email" required autocomplete="username" />
          </div>
          <div class="form-row">
            <label>Lozinka</label>
            <input type="password" name="password" required autocomplete="new-password" />
          </div>
          <button class="btn btn-primary" type="submit">Registriraj se</button>
        </form>
        <p class="small muted" style="margin-top:14px;">
          Već imaš račun? <a href="#/login">Prijavi se</a>
        </p>
      </div>
    `;

    container.querySelector("#registerForm").addEventListener("submit", async (e) => {
      e.preventDefault();
      const fd = new FormData(e.target);
      try {
        await App.api.users.register({
          name: fd.get("name"),
          surname: fd.get("surname"),
          email: fd.get("email"),
          password: fd.get("password"),
        });
        toast("Račun kreiran. Sad se prijavi.", "success");
        App.router.navigate("/login");
      } catch (err) {
        errorToast(err);
      }
    });
  }

  async function viewProfile(container) {
    const user = App.auth.currentUser();
    let profile;
    try {
      profile = await App.api.users.get(user.id);
    } catch (err) {
      container.innerHTML = `<div class="card empty-state">Greška: ${escapeHtml(err.message)}</div>`;
      return;
    }

    container.innerHTML = `
      <h1>Moj profil</h1>
      <div class="split two-col">
        <div class="section">
          <div class="card">
            <h3>Podaci o računu</h3>
            <form class="form" id="profileForm">
              <div class="form-row-inline">
                <div class="form-row"><label>Ime</label><input name="name" value="${escapeHtml(profile.name)}" required /></div>
                <div class="form-row"><label>Prezime</label><input name="surname" value="${escapeHtml(profile.surname)}" required /></div>
              </div>
              <div class="form-row"><label>Email</label><input type="email" name="email" value="${escapeHtml(profile.email)}" required /></div>
              <div class="form-row"><label>Nova lozinka</label><input type="password" name="password" required placeholder="Unesi lozinku za potvrdu promjena" /></div>
              <button class="btn btn-primary" type="submit">Spremi promjene</button>
            </form>
            <hr style="margin:16px 0;border-color:var(--border);" />
            <button class="btn btn-danger" id="deleteAccountBtn">Obriši račun</button>
          </div>
        </div>
        <div class="section">
          <h3>Moji upisani kolegiji</h3>
          <div id="enrolledArea"><div class="muted small">Učitavanje…</div></div>
        </div>
      </div>
    `;

    container.querySelector("#profileForm").addEventListener("submit", async (e) => {
      e.preventDefault();
      const fd = new FormData(e.target);
      try {
        await App.api.users.update(user.id, {
          name: fd.get("name"),
          surname: fd.get("surname"),
          email: fd.get("email"),
          password: fd.get("password"),
        });
        toast("Profil ažuriran.", "success");
      } catch (err) {
        errorToast(err);
      }
    });

    container.querySelector("#deleteAccountBtn").addEventListener("click", async () => {
      if (!confirmAction("Sigurno želiš trajno obrisati svoj račun?")) return;
      try {
        await App.api.users.delete(user.id);
        App.auth.clearToken();
        toast("Račun obrisan.", "success");
        App.router.navigate("/");
        App.main.renderNav();
      } catch (err) {
        errorToast(err);
      }
    });

    const enrolledArea = container.querySelector("#enrolledArea");
    try {
      const enrollments = await App.api.enrollments.byUser(user.id);
      if (!enrollments || !enrollments.length) {
        enrolledArea.innerHTML = `<div class="muted small">Nisi upisan/a ni na jedan kolegij. <a href="#/">Pregledaj kolegije</a>.</div>`;
      } else {
        const courses = await Promise.all(enrollments.map((e) => App.api.courses.get(e.courseId).catch(() => null)));
        enrolledArea.innerHTML = enrollments
          .map((e, i) => {
            const c = courses[i];
            return `
            <div class="card" style="margin-bottom:8px;display:flex;justify-content:space-between;align-items:center;">
              <div>
                <strong>${c ? escapeHtml(c.name) : "Kolegij #" + e.courseId}</strong>
                <div class="muted small">${c ? formatTimeSpan(c.timeToComplete) : ""}</div>
              </div>
              <div style="display:flex;gap:8px;align-items:center;">
                ${e.completed ? `<span class="pill pill-success">Završeno</span>` : `<span class="pill pill-muted">U tijeku</span>`}
                <a class="btn btn-sm" href="#/course/${e.courseId}">Otvori</a>
              </div>
            </div>`;
          })
          .join("");
      }
    } catch (err) {
      enrolledArea.innerHTML = `<div class="muted small">Greška: ${escapeHtml(err.message)}</div>`;
    }
  }

  window.App = window.App || {};
  window.App.views = window.App.views || {};
  window.App.views.login = viewLogin;
  window.App.views.register = viewRegister;
  window.App.views.profile = viewProfile;
})();
