const tableBody = document.getElementById("scheduledMatchesTable");
const toast = document.querySelector(".dashboard-toast");
let toastTimeoutId = null;
document.addEventListener("DOMContentLoaded", async () => {
  await loadScheduledMatches();

  document
    .querySelector("[data-autocomplete-root]")
    ?.addEventListener("autocomplete:confirm", (e) =>
      loadScheduledMatches(e.detail.term),
    );
});

tableBody.addEventListener("click", function (e) {
  if (e.target.classList.contains("dashboard-row-button-edit")) {
    const match = JSON.parse(e.target.dataset.match);
    editScheduledMatch(match);
  }
});

//injeted form actions
function fetchForm(callback) {
  dashboardSpinner.show();
  $.get("/scheduled-matches/form", function (html) {
    $("#form-container").html(html);
    $.validator.unobtrusive.parse("#scheduled-match-form");
    if (callback) callback();
  }).always(function () {
    dashboardSpinner.hide();
  });
}

function cancelForm() {
  $("#form-container").html("");
}

function setEditState(isEditing, label) {
  const form = document.getElementById("scheduled-match-form");
  const banner = document.getElementById("scheduled-match-edit-banner");

  if (!form || !banner) return;

  form.classList.toggle("dashboard-form--editing", isEditing);
  banner.hidden = !isEditing;
  banner.textContent = isEditing
    ? `Editing scheduled match: ${label || "Selected"}`
    : "";
}

function showToast(message) {
  if (!toast) return;
  toast.textContent = message;
  toast.classList.add("is-show");
  if (toastTimeoutId) {
    window.clearTimeout(toastTimeoutId);
  }
  toastTimeoutId = window.setTimeout(
    () => toast.classList.remove("is-show"),
    1800,
  );
}

function collectValidationMessages(responseJson) {
  if (!responseJson) return [];

  if (Array.isArray(responseJson)) {
    return responseJson
      .map((error) => {
        if (!error) return null;
        if (typeof error === "string") return error;
        return error.description || error.Description || null;
      })
      .filter(Boolean);
  }

  if (responseJson.errors) {
    return Object.values(responseJson.errors).flat();
  }

  if (typeof responseJson === "string") {
    return [responseJson];
  }

  return [];
}

function renderValidationSummary(formId, messages) {
  const form = document.getElementById(formId);
  if (!form) return;

  const summary = form.querySelector("[data-valmsg-summary]");
  if (!summary) return;

  summary.classList.remove("validation-summary-valid");
  summary.classList.add("validation-summary-errors");
  summary.innerHTML = "";

  if (!messages.length) {
    summary.classList.add("validation-summary-valid");
    summary.classList.remove("validation-summary-errors");
    return;
  }

  const list = document.createElement("ul");
  messages.forEach((message) => {
    const item = document.createElement("li");
    item.textContent = message;
    list.appendChild(item);
  });
  summary.appendChild(list);
}

//Read
function loadScheduledMatches(search = "") {
  dashboardSpinner.show();
  const url = search
    ? `/scheduled-matches/data?search=${encodeURIComponent(search)}`
    : "/scheduled-matches/data";
  $.ajax({
    url,
    method: "GET",
    success: function (matches) {
      let injectedHtml = "";
      matches.forEach((match) => {
        injectedHtml += `
                        <tr>
                            <td>${match.playingFieldName}</td>
                            <td>${match.partyDescription}</td>
                            <td>${match.matchDate}</td>
                            <td>
                                <button class="dashboard-row-button dashboard-row-button--edit dashboard-row-button-edit" data-match='${JSON.stringify(match)}'>Edit</button>
                                <button class="dashboard-row-button dashboard-row-button--danger" onclick="deleteScheduledMatch('${match.id}')">Delete</button>
                            </td>
                        </tr>
                        `;
      });
      tableBody.innerHTML = injectedHtml;
    },
    error: function () {
      return;
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Create
function openCreate() {
  fetchForm(() => {
    $("#scheduled-match-id").val("");
    setEditState(false);
  });
}

function submitForm() {
  const id = $("#scheduled-match-id").val();
  const url = id
    ? `/scheduled-matches/edit/${id}`
    : "/scheduled-matches/create";
  const method = "POST";

  if (!$("#scheduled-match-form").valid()) return;

  $.ajax({
    url: url,
    method: method,
    contentType: "application/json",
    data: JSON.stringify({
      id: $("#scheduled-match-id").val() || null,
      playingFieldId: $("#Form_PlayingFieldId").val(),
      partyId: $("#Form_PartyId").val(),
      matchDate: $("#Form_MatchDate").val(),
    }),
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      cancelForm();
      loadScheduledMatches();
      showToast(id ? "Updated!" : "Saved!");
    },
    error: function (xhr) {
      renderValidationSummary(
        "scheduled-match-form",
        collectValidationMessages(xhr.responseJSON),
      );
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Delete
function deleteScheduledMatch(id) {
  if (!confirm("Are you sure?")) return;

  $.ajax({
    url: `/scheduled-matches/delete/${id}`,
    method: "DELETE",
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      loadScheduledMatches();
      showToast("Deleted!");
    },
    error: function (xhr) {
      console.log(xhr.responseJSON);
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Edit
function editScheduledMatch(match) {
  fetchForm(() => {
    $("#scheduled-match-id").val(match.id);
    $("#Form_PlayingFieldId").val(match.playingFieldId);
    $("#Form_PartyId").val(match.partyId);
    $("#Form_MatchDate").val(match.matchDate);
    setEditState(true, match.playingFieldName);
  });
}
