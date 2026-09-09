// Exam-taking flow: start attempt -> answer questions (locked in one at a time, since the
// API only lets Admins delete an ExamQuestionAnswer, i.e. students can't change their mind
// once an answer is posted) -> submit attempt -> show score.
(function () {
  const { escapeHtml, formatTimeSpan, errorToast, toast, confirmAction } = App.ui;

  async function viewExam(container, { id }) {
    const user = App.auth.currentUser();
    container.innerHTML = `<div class="empty-state">Učitavanje ispita…</div>`;

    let exam, questions;
    try {
      [exam, questions] = await Promise.all([App.api.exams.get(id), App.api.questions.byExam(id)]);
    } catch (err) {
      container.innerHTML = `<div class="card empty-state">Greška: ${escapeHtml(err.message)}</div>`;
      return;
    }

    const state = {
      attempt: null,
      lockedQuestionIds: new Set(),
    };

    function renderShell() {
      container.innerHTML = `
        <div class="breadcrumbs"><a href="#/course/${exam.courseId}">Natrag na kolegij</a></div>
        <div class="page-header">
          <div>
            <h1>${escapeHtml(exam.title)}</h1>
            <div class="muted">Vrijeme za rješavanje: ${formatTimeSpan(exam.allotedTime)} · ${questions.length} pitanja</div>
          </div>
          <div id="examActions"></div>
        </div>
        <div id="examBody"></div>
      `;
    }

    function renderStart() {
      renderShell();
      const actions = container.querySelector("#examActions");
      actions.innerHTML = `<button class="btn btn-primary" id="startBtn">Pokreni ispit</button>`;
      container.querySelector("#examBody").innerHTML = `<div class="card empty-state">Klikni "Pokreni ispit" kad budeš spreman/na. Nakon što odabereš odgovor za pitanje, ne možeš ga naknadno promijeniti.</div>`;

      actions.querySelector("#startBtn").addEventListener("click", async () => {
        try {
          state.attempt = await App.api.examAttempts.create({ examId: Number(id), userId: user.id });
          renderQuestions();
        } catch (err) {
          errorToast(err);
        }
      });
    }

    function renderQuestions() {
      renderShell();
      const actions = container.querySelector("#examActions");
      actions.innerHTML = `<button class="btn btn-primary" id="submitBtn">Predaj ispit</button>`;
      const body = container.querySelector("#examBody");

      if (!questions.length) {
        body.innerHTML = `<div class="card empty-state">Ovaj ispit nema pitanja.</div>`;
      } else {
        body.innerHTML = questions
          .map((q, idx) => {
            // Single choice (radio) for every question type: the API has no endpoint letting a
            // student un-pick an answer, so allowing multi-select checkboxes would let mistakes
            // become permanent. Radios keep exactly one POST per question.
            return `
            <div class="question-card" data-question="${q.id}">
              <h3>${idx + 1}. ${escapeHtml(q.text)}</h3>
              <div class="answers">
                ${(q.answers || [])
                  .map(
                    (a) => `
                  <label class="answer-option">
                    <input type="radio" name="q-${q.id}" value="${a.id}" />
                    ${escapeHtml(a.option)}
                  </label>`
                  )
                  .join("")}
              </div>
              <button class="btn btn-sm" data-lock="${q.id}" style="margin-top:8px;">Zaključaj odgovor</button>
              <div class="muted small" data-status="${q.id}" style="margin-top:6px;"></div>
            </div>
          `;
          })
          .join("");

        body.querySelectorAll("[data-lock]").forEach((btn) => {
          btn.addEventListener("click", async () => {
            const qid = btn.dataset.lock;
            const card = body.querySelector(`[data-question="${qid}"]`);
            const checked = card.querySelectorAll(`input[name="q-${qid}"]:checked`);
            if (!checked.length) {
              toast("Odaberi odgovor prije zaključavanja.", "error");
              return;
            }
            try {
              for (const input of checked) {
                await App.api.examQuestionAnswers.create({
                  answerId: Number(input.value),
                  questionId: Number(qid),
                  attemptId: state.attempt.id,
                });
              }
              state.lockedQuestionIds.add(qid);
              card.classList.add("locked");
              card.querySelectorAll("input").forEach((i) => (i.disabled = true));
              btn.remove();
              card.querySelector(`[data-status="${qid}"]`).textContent = "Odgovor zaključan.";
            } catch (err) {
              errorToast(err);
            }
          });
        });
      }

      actions.querySelector("#submitBtn").addEventListener("click", async () => {
        if (state.lockedQuestionIds.size < questions.length) {
          if (!confirmAction("Nisi odgovorio/la na sva pitanja. Ipak predati ispit?")) return;
        } else if (!confirmAction("Predati ispit? Ovo se ne može poništiti.")) {
          return;
        }
        try {
          const result = await App.api.examAttempts.submit(state.attempt.id);
          renderResult(result);
        } catch (err) {
          errorToast(err);
        }
      });
    }

    function renderResult(result) {
      renderShell();
      container.querySelector("#examActions").innerHTML = "";
      container.querySelector("#examBody").innerHTML = `
        <div class="card" style="text-align:center;">
          <h2>Rezultat</h2>
          <p style="font-size:2rem;margin:6px 0;">${Number(result.score).toFixed(1)}</p>
          <p>${result.passed ? '<span class="pill pill-success">Položeno</span>' : '<span class="pill pill-danger">Nije položeno</span>'}</p>
          <a href="#/course/${exam.courseId}" class="btn btn-primary" style="margin-top:10px;">Natrag na kolegij</a>
        </div>
      `;
    }

    renderStart();
  }

  window.App = window.App || {};
  window.App.views = window.App.views || {};
  window.App.views.exam = viewExam;
})();
