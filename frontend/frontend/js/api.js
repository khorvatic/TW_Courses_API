// Thin fetch wrapper around the TW_Courses_API + typed helpers per resource.
// Mirrors the routes/DTOs found in the API's Controllers and Application/DTO folders.
(function () {
  const DEFAULT_BASE = "http://localhost:5019/api";

  function getBaseUrl() {
    return localStorage.getItem("tw_api_base") || DEFAULT_BASE;
  }

  function setBaseUrl(url) {
    localStorage.setItem("tw_api_base", url.replace(/\/+$/, ""));
  }

  class ApiError extends Error {
    constructor(message, status, data) {
      super(message);
      this.status = status;
      this.data = data;
    }
  }

  // Flattens ASP.NET ValidationProblemDetails ({errors: {Field: ["msg"]}}) or {message} into one string.
  function extractMessage(data, fallback) {
    if (!data) return fallback;
    if (typeof data === "string") return data;
    if (data.message) return data.message;
    if (data.title && data.errors) {
      const parts = Object.values(data.errors).flat();
      return parts.length ? parts.join(" ") : data.title;
    }
    if (data.title) return data.title;
    return fallback;
  }

  async function request(path, { method = "GET", body, auth = true } = {}) {
    const headers = { Accept: "application/json" };
    if (body !== undefined) headers["Content-Type"] = "application/json";

    if (auth) {
      const token = App.auth.getToken();
      if (token) headers["Authorization"] = "Bearer " + token;
    }

    let res;
    try {
      res = await fetch(getBaseUrl() + path, {
        method,
        headers,
        body: body !== undefined ? JSON.stringify(body) : undefined,
      });
    } catch (networkErr) {
      throw new ApiError(
        "Ne mogu se spojiti na API na " + getBaseUrl() + ". Provjeri je li API pokrenut i je li CORS ispravno postavljen.",
        0,
        null
      );
    }

    const text = await res.text();
    let data = null;
    if (text) {
      try {
        data = JSON.parse(text);
      } catch {
        data = text;
      }
    }

    if (!res.ok) {
      if (res.status === 401) App.auth.clearToken();
      throw new ApiError(extractMessage(data, `Greška ${res.status}`), res.status, data);
    }

    return data;
  }

  const api = {
    getBaseUrl,
    setBaseUrl,
    ApiError,

    auth: {
      login: (dto) => request("/auth", { method: "POST", body: dto, auth: false }),
    },

    users: {
      register: (dto) => request("/user", { method: "POST", body: dto, auth: false }),
      get: (id) => request(`/user/${id}`),
      getAll: () => request("/user"),
      getByEmail: (email) => request(`/user/by-email?email=${encodeURIComponent(email)}`),
      update: (id, dto) => request(`/user/${id}`, { method: "PUT", body: dto }),
      delete: (id) => request(`/user/${id}`, { method: "DELETE" }),
    },

    roles: {
      getAll: () => request("/role"),
      get: (id) => request(`/role/${id}`),
      create: (dto) => request("/role", { method: "POST", body: dto }),
      delete: (id) => request(`/role/${id}`, { method: "DELETE" }),
    },

    userRoles: {
      getAll: () => request("/userrole"),
      byUser: (userId) => request(`/userrole/user/${userId}/roles`),
      assign: (dto) => request("/userrole", { method: "POST", body: dto }),
      remove: (userId, roleId) => request(`/userrole?userId=${userId}&roleId=${roleId}`, { method: "DELETE" }),
    },

    courses: {
      getAll: () => request("/course", { auth: false }),
      get: (id) => request(`/course/${id}`, { auth: false }),
      create: (dto) => request("/course", { method: "POST", body: dto }),
      update: (id, dto) => request(`/course/${id}`, { method: "PUT", body: dto }),
      delete: (id) => request(`/course/${id}`, { method: "DELETE" }),
    },

    chapters: {
      byCourse: (courseId) => request(`/chapter/course/${courseId}`),
      get: (id) => request(`/chapter/${id}`),
      create: (courseId, dto) => request(`/chapter/${courseId}`, { method: "POST", body: dto }),
      update: (id, dto) => request(`/chapter/${id}`, { method: "PUT", body: dto }),
      delete: (id) => request(`/chapter/${id}`, { method: "DELETE" }),
    },

    enrollments: {
      get: (id) => request(`/enrolledcourse/${id}`),
      byUser: (userId) => request(`/enrolledcourse/user/${userId}`),
      enroll: (dto) => request("/enrolledcourse", { method: "POST", body: dto }),
      complete: (userId, courseId) =>
        request(`/enrolledcourse/complete?userId=${userId}&courseId=${courseId}`, { method: "PUT" }),
    },

    exams: {
      getAll: () => request("/exam"),
      get: (id) => request(`/exam/${id}`),
      byCourse: (courseId) => request(`/exam/course/${courseId}`),
      create: (dto) => request("/exam", { method: "POST", body: dto }),
      update: (id, dto) => request(`/exam/${id}`, { method: "PUT", body: dto }),
      delete: (id) => request(`/exam/${id}`, { method: "DELETE" }),
    },

    questions: {
      get: (id) => request(`/question/${id}`),
      byExam: (examId) => request(`/question/exam/${examId}`),
      create: (dto) => request("/question", { method: "POST", body: dto }),
      update: (id, dto) => request(`/question/${id}`, { method: "PUT", body: dto }),
      delete: (id) => request(`/question/${id}`, { method: "DELETE" }),
    },

    answers: {
      create: (dto) => request("/answer", { method: "POST", body: dto }),
      update: (id, dto) => request(`/answer/${id}`, { method: "PUT", body: dto }),
      delete: (id) => request(`/answer/${id}`, { method: "DELETE" }),
    },

    examAttempts: {
      getAll: () => request("/examattempt"),
      get: (id) => request(`/examattempt/${id}`),
      byExam: (examId) => request(`/examattempt/exam/${examId}`),
      byUser: (userId) => request(`/examattempt/user/${userId}`),
      create: (dto) => request("/examattempt", { method: "POST", body: dto }),
      submit: (attemptId) => request(`/examattempt/${attemptId}`, { method: "PUT" }),
    },

    examQuestionAnswers: {
      create: (dto) => request("/examquestionanswer", { method: "POST", body: dto }),
    },

    reviews: {
      getAll: () => request("/review"),
      get: (id) => request(`/review/${id}`),
      byCourse: (courseId) => request(`/review/course/${courseId}`),
      byUser: (userId) => request(`/review/user/${userId}`),
      create: (dto) => request("/review", { method: "POST", body: dto }),
      update: (id, dto) => request(`/review/${id}`, { method: "PUT", body: dto }),
      delete: (id) => request(`/review/${id}`, { method: "DELETE" }),
    },
  };

  window.App = window.App || {};
  window.App.api = api;
})();
