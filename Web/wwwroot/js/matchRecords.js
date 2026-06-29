const tableBody = document.getElementById("matchRecordsTable");
const toast = document.querySelector(".dashboard-toast");
let toastTimeoutId = null;
let matchPlayersSelect = null;
let ratingEntries = [];
document.addEventListener("DOMContentLoaded", async () => {
  await loadMatchRecords();

  document
    .querySelector("[data-autocomplete-root]")
    ?.addEventListener("autocomplete:confirm", (e) =>
      loadMatchRecords(e.detail.term),
    );
});

tableBody.addEventListener("click", function (e) {
  if (e.target.classList.contains("dashboard-row-button-edit")) {
    const record = JSON.parse(e.target.dataset.record);
    editRecord(record);
  }
});

//injeted form actions
function fetchForm(recordId, callback) {
  dashboardSpinner.show();
  const url = recordId ? `/matches/form?id=${recordId}` : "/matches/form";
  $.get(url, function (html) {
    $("#form-container").html(html);
    $.validator.unobtrusive.parse("#match-record-form");
    matchPlayersSelect = new HegelMultiSelect("hms-match-players");
    initRatings();
    initPlayerStats();
    if (window.initDatetimePickers) {
      initDatetimePickers(document.getElementById("match-record-form"));
    }
    if (callback) callback();
  }).always(function () {
    dashboardSpinner.hide();
  });
}

function cancelForm() {
  $("#form-container").html("");
}

function setEditState(isEditing, label) {
  const form = document.getElementById("match-record-form");
  const banner = document.getElementById("match-record-edit-banner");

  if (!form || !banner) return;

  form.classList.toggle("dashboard-form--editing", isEditing);
  banner.hidden = !isEditing;
  banner.textContent = isEditing
    ? `Editing match record: ${label || "Selected"}`
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
function loadMatchRecords(search = "") {
  dashboardSpinner.show();
  const url = search
    ? `/api/match-records?search=${encodeURIComponent(search)}`
    : "/api/match-records";
  $.ajax({
    url,
    method: "GET",
    success: function (records) {
      let injectedHtml = "";
      records.forEach((record) => {
        injectedHtml += `
                        <tr>
                            <td>${record.goalsTeamA} - ${record.goalsTeamB}</td>
                            <td>${record.matchHeld}</td>
                            <td>${record.wasMatchHeld}</td>
                            <td>
                                <button class="dashboard-row-button dashboard-row-button--edit dashboard-row-button-edit" data-record='${JSON.stringify(record)}'>Edit</button>
                                <button class="dashboard-row-button dashboard-row-button--danger" onclick="deleteRecord('${record.id}')">Delete</button>
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
  fetchForm(null, () => {
    $("#match-record-id").val("");
    setEditState(false);
  });
}

function submitForm() {
  const id = $("#match-record-id").val();
  const url = id ? `/api/match-records/${id}` : "/api/match-records";
    const method = id ? "PUT" : "POST";

  if (!$("#match-record-form").valid()) return;

  $.ajax({
    url: url,
    method: method,
    contentType: "application/json",
    data: JSON.stringify({
      id: $("#match-record-id").val() || null,
      wasMatchHeld: $("#Form_WasMatchHeld").is(":checked"),
      matchHeld: $("#Form_MatchHeld").val(),
      playingFieldId: $("#Form_PlayingFieldId").val(),
      goalsTeamA: $("#Form_GoalsTeamA").val(),
      goalsTeamB: $("#Form_GoalsTeamB").val(),
      matchPlayerIds: matchPlayersSelect
        ? matchPlayersSelect.getData().ids
        : [],
      playerRatings: ratingEntries,
      matchPlayerStats: collectPlayerStats(),
    }),
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      cancelForm();
      loadMatchRecords();
      showToast(id ? "Ažurirano!" : "Spremljeno!");
    },
    error: function (xhr) {
      showErrorModal(collectValidationMessages(xhr.responseJSON));
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Delete
function deleteRecord(id) {
  if (!confirm("Jeste li sigurni?")) return;

  $.ajax({
    url: `/api/match-records/${id}`,
    method: "DELETE",
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      loadMatchRecords();
      showToast("Izbrisano!");
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
function editRecord(record) {
  fetchForm(record.id, () => {
    $("#match-record-id").val(record.id);
    $("#Form_WasMatchHeld").prop("checked", record.wasMatchHeld);
    $("#Form_MatchHeld").val(record.matchHeld);
    $("#Form_PlayingFieldId").val(record.playingFieldId);
    $("#Form_GoalsTeamA").val(record.goalsTeamA);
    $("#Form_GoalsTeamB").val(record.goalsTeamB);
    setEditState(true, record.playingFieldName);
    updateRatingOptions();
  });
}

function initRatings() {
  ratingEntries = [];
  const ratingSection = document.getElementById("match-rating-section");
  const ratingList = document.getElementById("rating-list");
  const addButton = document.getElementById("add-rating");
  const giverSelect = document.getElementById("rating-giver");
  const receiverSelect = document.getElementById("rating-receiver");
  const ratingInput = document.getElementById("rating-value");

  if (
    !ratingSection ||
    !ratingList ||
    !giverSelect ||
    !receiverSelect ||
    !ratingInput
  ) {
    return;
  }

  ratingList.querySelectorAll(".dashboard-rating-chip").forEach((chip) => {
    const giverId = chip.dataset.giverId;
    const receiverId = chip.dataset.receiverId;
    const ratingValue = Number(chip.dataset.rating);
    if (!giverId || !receiverId || Number.isNaN(ratingValue)) return;
    ratingEntries.push({
      playerGivingRatingId: giverId,
      playerReceivingRatingId: receiverId,
      rating: ratingValue,
    });
  });

  const container = document.getElementById("hms-match-players");
  if (container) {
    container.addEventListener("hms:change", updateRatingOptions);
  }

  document
    .getElementById("Form_WasMatchHeld")
    ?.addEventListener("change", updateRatingOptions);

  ratingList.addEventListener("click", (event) => {
    if (!event.target.classList.contains("dashboard-chip-remove")) return;
    const chip = event.target.closest(".dashboard-rating-chip");
    if (!chip) return;
    const giverId = chip.dataset.giverId;
    const receiverId = chip.dataset.receiverId;
    const ratingValue = Number(chip.dataset.rating);
    ratingEntries = ratingEntries.filter(
      (entry) =>
        entry.playerGivingRatingId !== giverId ||
        entry.playerReceivingRatingId !== receiverId ||
        entry.rating !== ratingValue,
    );
    chip.remove();
  });

  if (addButton) {
    addButton.addEventListener("click", () =>
      addRatingEntry(giverSelect, receiverSelect, ratingInput, ratingList),
    );
  }

  updateRatingOptions();
}

function updateRatingOptions() {
  const ratingSection = document.getElementById("match-rating-section");
  const giverSelect = document.getElementById("rating-giver");
  const receiverSelect = document.getElementById("rating-receiver");
  if (!ratingSection || !giverSelect || !receiverSelect) return;

  const selectedIds = matchPlayersSelect
    ? matchPlayersSelect.getData().ids
    : [];
  const wasMatchHeld =
    document.getElementById("Form_WasMatchHeld")?.checked ?? false;
  const showRatings = wasMatchHeld && selectedIds.length >= 2;
  ratingSection.hidden = !showRatings;

  if (!showRatings) {
    const ratingList = document.getElementById("rating-list");
    if (ratingList) {
      ratingList.innerHTML = "";
    }
    ratingEntries = [];
    return;
  }

  const available = matchPlayersSelect ? matchPlayersSelect.availableItems : [];
  const selectedItems = available.filter((item) =>
    selectedIds.includes(item.id),
  );

  const buildOptions = (select) => {
    select.innerHTML = "";
    selectedItems.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.id;
      option.textContent = item.displayName;
      select.appendChild(option);
    });
  };

  buildOptions(giverSelect);
  buildOptions(receiverSelect);
}

function addRatingEntry(giverSelect, receiverSelect, ratingInput, ratingList) {
  const giverId = giverSelect.value;
  const receiverId = receiverSelect.value;
  const ratingValue = Number(ratingInput.value);

  if (!giverId || !receiverId || giverId === receiverId) return;
  if (Number.isNaN(ratingValue) || ratingValue < 1 || ratingValue > 5) return;

  ratingEntries.push({
    playerGivingRatingId: giverId,
    playerReceivingRatingId: receiverId,
    rating: ratingValue,
  });

  const giverName = giverSelect.options[giverSelect.selectedIndex]?.textContent;
  const receiverName =
    receiverSelect.options[receiverSelect.selectedIndex]?.textContent;

  const chip = document.createElement("div");
  chip.className = "dashboard-rating-chip";
  chip.dataset.giverId = giverId;
  chip.dataset.receiverId = receiverId;
  chip.dataset.rating = ratingValue;
  chip.innerHTML = `
    <span>${giverName} -> ${receiverName}: ${ratingValue}</span>
    <button type="button" class="dashboard-chip-remove" aria-label="Remove rating">X</button>
  `;

  ratingList.appendChild(chip);
  ratingInput.value = "";
}

function initPlayerStats() {
  const section = document.getElementById("match-player-stats-section");
  if (!section) return;

  const rows = section.querySelectorAll("tbody tr");
  if (!rows.length) section.setAttribute("hidden", "hidden");

  document
    .getElementById("hms-match-players")
    ?.addEventListener("hms:change", () => refreshPlayerStatsRows(section));
}

function refreshPlayerStatsRows(section) {
  const selected = matchPlayersSelect ? matchPlayersSelect.selectedItems : {};
  const members = Object.entries(selected); // [[id, displayName], ...]

  if (!members.length) {
    section.setAttribute("hidden", "hidden");
    return;
  }

  const tbody = section.querySelector("tbody");
  const incomingIds = new Set(members.map(([id]) => id));

  // remove rows for deselected players
  tbody.querySelectorAll("tr").forEach((row) => {
    if (!incomingIds.has(row.dataset.playerId)) row.remove();
  });

  // add rows for newly selected players (preserve existing so values aren't reset)
  const existingIds = new Set(
    [...tbody.querySelectorAll("tr")].map((r) => r.dataset.playerId),
  );
  members.forEach(([id, name]) => {
    if (existingIds.has(id)) return;
    const tr = document.createElement("tr");
    tr.dataset.playerId = id;
    tr.innerHTML = `
      <td>${name}</td>
      <td><input type="number" class="player-goals" value="0" min="0" /></td>
      <td><input type="number" class="player-assists" value="0" min="0" /></td>
    `;
    tbody.appendChild(tr);
  });

  section.removeAttribute("hidden");
}

function collectPlayerStats() {
  const section = document.getElementById("match-player-stats-section");
  if (!section || section.hasAttribute("hidden")) return [];

  const stats = [];
  section.querySelectorAll("tbody tr").forEach((row) => {
    const playerId = row.dataset.playerId;
    const goals = Number(row.querySelector(".player-goals")?.value) || 0;
    const assists = Number(row.querySelector(".player-assists")?.value) || 0;
    if (playerId) stats.push({ playerId, goals, assists });
  });
  return stats;
}
