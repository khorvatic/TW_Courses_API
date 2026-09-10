// Bootstraps the app: registers routes, renders the nav bar, wires the API-base settings panel.
(function () {
  function renderNav() {
    const nav = document.getElementById("nav");
    const user = App.auth.currentUser();

    let links = `<a href="#/">Kolegiji</a>`;

    if (user) {
      links += `<a href="#/profile">Moj profil</a>`;
      if (App.auth.hasRole("Admin", "Instructor")) {
        links += `<a href="#/admin">Admin panel</a>`;
      }
      links += `<span class="spacer"></span>`;
      links += `<span class="badge-role">${App.ui.escapeHtml(user.email || "")}${
        user.roles.length ? " · " + App.ui.escapeHtml(user.roles.join(", ")) : ""
      }</span>`;
      links += `<button class="linklike" id="logoutBtn">Odjava</button>`;
    } else {
      links += `<span class="spacer"></span>`;
      links += `<a href="#/login">Prijava</a>`;
      links += `<a href="#/register">Registracija</a>`;
    }

    nav.innerHTML = links;

    const current = location.hash.slice(1) || "/";
    nav.querySelectorAll("a[href^='#/']").forEach((a) => {
      if (a.getAttribute("href") === "#" + current) a.classList.add("active");
    });

    const logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
      logoutBtn.addEventListener("click", () => {
        App.auth.clearToken();
        App.ui.toast("Odjavljen/a.", "success");
        App.router.navigate("/");
        renderNav();
      });
    }
  }

  function setupSettingsPanel() {
    const btn = document.getElementById("settingsBtn");
    const panel = document.getElementById("settingsPanel");
    const input = document.getElementById("apiBaseInput");
    const saveBtn = document.getElementById("apiBaseSave");

    input.value = App.api.getBaseUrl();

    btn.addEventListener("click", () => panel.classList.toggle("hidden"));
    saveBtn.addEventListener("click", () => {
      const value = input.value.trim();
      if (!value) return;
      App.api.setBaseUrl(value);
      App.ui.toast("API URL spremljen.", "success");
      panel.classList.add("hidden");
      App.router.resolve();
    });
  }

  function registerRoutes() {
    const r = App.router;
    const v = App.views;
    r.on("/", v.home);
    r.on("/course/:id", v.courseDetail);
    r.on("/login", v.login);
    r.on("/register", v.register);
    r.on("/profile", v.profile, { requireAuth: true });
    r.on("/exam/:id", v.exam, { requireAuth: true });
    r.on("/admin", v.admin, { requireAuth: true, roles: ["Admin", "Instructor"] });
  }

  document.addEventListener("DOMContentLoaded", () => {
    registerRoutes();
    setupSettingsPanel();
    renderNav();
    App.router.init();
  });

  window.App = window.App || {};
  window.App.main = { renderNav };
})();
