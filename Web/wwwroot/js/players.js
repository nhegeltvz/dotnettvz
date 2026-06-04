const tableBody = document.getElementById("playerTable");
const toast = document.querySelector(".dashboard-toast");
let toastTimeoutId = null;
document.addEventListener("DOMContentLoaded", async () => {
  await loadPlayers();

  document
    .querySelector("[data-autocomplete-root]")
    ?.addEventListener("autocomplete:confirm", (e) =>
      loadPlayers(e.detail.term),
    );
});

tableBody.addEventListener("click", function (e) {
  if (e.target.classList.contains("dashboard-row-button-edit")) {
    const player = JSON.parse(e.target.dataset.player);
    editPlayer(player);
  }
});

//injeted form actions
function fetchForm(callback) {
  $.get("/players/form", function (html) {
    $("#form-container").html(html);
    $.validator.unobtrusive.parse("#player-form");
    if (callback) callback();
  });
}

function cancelForm() {
  $("#form-container").html("");
}

function setEditState(isEditing, label) {
  const form = document.getElementById("player-form");
  const banner = document.getElementById("player-edit-banner");

  if (!form || !banner) return;

  form.classList.toggle("dashboard-form--editing", isEditing);
  banner.hidden = !isEditing;
  banner.textContent = isEditing
    ? `Editing player: ${label || "Selected"}`
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
function loadPlayers(search = "") {
  dashboardSpinner.show();
  const url = search
    ? `/players/data?search=${encodeURIComponent(search)}`
    : "/players/data";
  $.ajax({
    url,
    method: "GET",
    success: function (players) {
      dashboardSpinner.hide();
      injectHtmlToTable(players, tableBody);
    },
    error: function (xhr) {
      dashboardSpinner.hide();
    },
  });
}

function injectHtmlToTable(players, tableBody) {
  let injectedHtml = "";

  players.forEach((player) => {
    injectedHtml += `
                    <tr>
                        <td>${player.username}</td>
                        <td>${player.email}</td>
                        <td>
                            <button class="dashboard-row-button dashboard-row-button--danger dashboard-row-button-delete" onclick="deletePlayer('${player.id}')">Delete</button>
                            <button class="dashboard-row-button dashboard-row-button--edit dashboard-row-button-edit" data-player='${JSON.stringify(player)}'>Edit</button>
                        </td>
                    </tr>
                    `;
  });

  tableBody.innerHTML = injectedHtml;
}

//Create
function openCreate() {
  fetchForm(() => {
    $("#player-id").val("");
    setEditState(false);
  });
}

function submitForm() {
  const id = $("#player-id").val();
  const url = id ? `/players/edit/${id}` : "/players/create";
  const method = id ? "PUT" : "POST";

  if (!$("#player-form").valid()) return;

  $.ajax({
    url: url,
    method: method,
    contentType: "application/json",
    data: JSON.stringify({
      id: $("#player-id").val() || null,
      username: $("#Username").val(),
      email: $("#Email").val(),
      password: $("#Password").val() || null,
      oib: $("#OIB").val(),
      jmbg: $("#JMBG").val(),
      bio: $("#Bio").val(),
      age: $("#Age").val(),
      preferredPosition: $("#PreferredPosition").val(),
    }),
    success: function () {
      cancelForm();
      loadPlayers();
      showToast(id ? "Updated!" : "Saved!");
    },
    error: function (xhr) {
      showErrorModal(collectValidationMessages(xhr.responseJSON));
    },
  });
}

//Delete
function deletePlayer(id) {
  if (!confirm("Are you sure?")) return;

  $.ajax({
    url: `/players/delete/${id}`,
    method: "DELETE",
    success: function () {
      loadPlayers();
      showToast("Deleted!");
    },
    error: function (xhr) {
      console.log(xhr.responseJSON);
    },
  });
}

//Edit
function editPlayer(player) {
  fetchForm(() => {
    $("#player-id").val(player.id);
    $("#Username").val(player.username);
    $("#Email").val(player.email);
    $("#OIB").val(player.oib);
    $("#JMBG").val(player.jmbg);
    $("#Bio").val(player.bio);
    $("#Age").val(player.age);
    $("#PreferredPosition").val(player.preferredPosition);
    setEditState(true, player.username);
  });
}
