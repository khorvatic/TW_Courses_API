// Minimal hash-based router: '#/course/12' -> matches '/course/:id' -> handler(container, {id:'12'})
(function () {
  const routes = [];

  function on(pattern, handler, opts = {}) {
    const paramNames = [];
    const regex = new RegExp(
      "^" +
        pattern
          .split("/")
          .map((seg) => {
            if (seg.startsWith(":")) {
              paramNames.push(seg.slice(1));
              return "([^/]+)";
            }
            return seg.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
          })
          .join("/") +
        "$"
    );
    routes.push({ regex, paramNames, handler, opts });
  }

  function currentPath() {
    const hash = location.hash || "#/";
    return hash.slice(1) || "/";
  }

  function navigate(path) {
    location.hash = "#" + path;
  }

  async function resolve() {
    const path = currentPath();
    const app = document.getElementById("app");

    for (const route of routes) {
      const match = route.regex.exec(path);
      if (match) {
        const params = {};
        route.paramNames.forEach((name, i) => (params[name] = decodeURIComponent(match[i + 1])));

        if (route.opts.requireAuth && !App.auth.isAuthenticated()) {
          App.ui.toast("Prijavi se za pristup ovoj stranici.", "error");
          navigate("/login");
          return;
        }
        if (route.opts.roles && !App.auth.hasRole(...route.opts.roles)) {
          app.innerHTML = `<div class="card empty-state">Nemaš ovlasti za pristup ovoj stranici.</div>`;
          return;
        }

        app.innerHTML = "";
        window.scrollTo(0, 0);
        try {
          await route.handler(app, params);
        } catch (err) {
          console.error(err);
          app.innerHTML = `<div class="card empty-state">Greška pri učitavanju stranice: ${App.ui.escapeHtml(err.message)}</div>`;
        }
        App.main.renderNav();
        return;
      }
    }

    app.innerHTML = `<div class="card empty-state">Stranica nije pronađena. <a href="#/">Natrag na naslovnicu</a></div>`;
  }

  function init() {
    window.addEventListener("hashchange", resolve);
    resolve();
  }

  window.App = window.App || {};
  window.App.router = { on, navigate, init, resolve };
})();
