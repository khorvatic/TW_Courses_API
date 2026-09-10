// Session handling: stores the raw JWT and derives user id / email / roles from its payload.
// The API signs claims with their .NET long-form URIs (see AuthService.GenerateToken), so we
// check both the short and long claim names when reading the payload back out.
(function () {
  const TOKEN_KEY = "tw_token";
  const ROLE_CLAIM_URIS = [
    "role",
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
  ];

  function base64UrlDecode(str) {
    let s = str.replace(/-/g, "+").replace(/_/g, "/");
    while (s.length % 4) s += "=";
    try {
      return decodeURIComponent(
        atob(s)
          .split("")
          .map((c) => "%" + c.charCodeAt(0).toString(16).padStart(2, "0"))
          .join("")
      );
    } catch {
      return null;
    }
  }

  function decode(token) {
    if (!token) return null;
    const parts = token.split(".");
    if (parts.length !== 3) return null;
    const json = base64UrlDecode(parts[1]);
    if (!json) return null;
    try {
      return JSON.parse(json);
    } catch {
      return null;
    }
  }

  function getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  function setToken(token) {
    localStorage.setItem(TOKEN_KEY, token);
  }

  function clearToken() {
    localStorage.removeItem(TOKEN_KEY);
  }

  function currentUser() {
    const token = getToken();
    if (!token) return null;
    const payload = decode(token);
    if (!payload) return null;

    if (payload.exp && Date.now() >= payload.exp * 1000) {
      clearToken();
      return null;
    }

    let roles = [];
    for (const key of ROLE_CLAIM_URIS) {
      if (payload[key]) {
        roles = Array.isArray(payload[key]) ? payload[key] : [payload[key]];
        break;
      }
    }

    const id = parseInt(payload.sub || payload.nameid || payload.id, 10);

    return {
      id: Number.isNaN(id) ? null : id,
      email: payload.email || null,
      roles,
    };
  }

  function isAuthenticated() {
    return !!currentUser();
  }

  function hasRole(...roles) {
    const user = currentUser();
    if (!user) return false;
    return roles.some((r) => user.roles.includes(r));
  }

  function isAdmin() {
    return hasRole("Admin");
  }

  function isInstructor() {
    return hasRole("Instructor");
  }

  window.App = window.App || {};
  window.App.auth = {
    getToken,
    setToken,
    clearToken,
    currentUser,
    isAuthenticated,
    hasRole,
    isAdmin,
    isInstructor,
  };
})();
