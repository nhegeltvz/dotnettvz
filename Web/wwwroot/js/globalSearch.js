(function () {
  const root = document.querySelector("[data-global-search-root]");
  if (!root) return;

  const input = root.querySelector(".global-search__input");
  const results = root.querySelector(".global-search__results");
  const clearBtn = root.querySelector(".global-search__clear");

  let debounceTimer = null;

  const endpoints = [
    {
      url: (q) => `/api/players?search=${encodeURIComponent(q)}`,
      label: "Players",
      getName: (item) => item.username,
      getHref: (item) => `/players/details/${item.id}`,
    },
    {
      url: (q) => `/api/playing-fields?search=${encodeURIComponent(q)}`,
      label: "Stadiums",
      getName: (item) => item.name,
      getHref: (item) => `/stadiums/details/${item.id}`,
    },
    {
      url: (q) => `/api/parties?search=${encodeURIComponent(q)}`,
      label: "Parties",
      getName: (item) => item.partyDescription,
      getHref: (item) => `/parties/details/${item.id}`,
    },
  ];

  function showResults(groups) {
    results.innerHTML = "";

    const hasAny = groups.some((g) => g.items.length > 0);
    if (!hasAny) {
      results.innerHTML =
        '<p class="global-search__empty">No results found.</p>';
      results.hidden = false;
      return;
    }

    groups.forEach(({ label, items, getHref, getName }) => {
      if (!items.length) return;

      const groupEl = document.createElement("div");
      groupEl.className = "global-search__group";

      const groupLabel = document.createElement("span");
      groupLabel.className = "global-search__group-label";
      groupLabel.textContent = label;
      groupEl.appendChild(groupLabel);

      items.slice(0, 5).forEach((item) => {
        const a = document.createElement("a");
        a.className = "global-search__option";
        a.href = getHref(item);
        a.textContent = getName(item);
        groupEl.appendChild(a);
      });

      results.appendChild(groupEl);
    });

    results.hidden = false;
  }

  function hideResults() {
    results.hidden = true;
    results.innerHTML = "";
  }

  async function search(query) {
    const fetches = endpoints.map(({ url, label, getName, getHref }) =>
      fetch(url(query))
        .then((r) => r.json())
        .then((items) => ({ label, items, getName, getHref }))
        .catch(() => ({ label, items: [], getName, getHref }))
    );

    const groups = await Promise.all(fetches);
    showResults(groups);
  }

  input.addEventListener("input", () => {
    const q = input.value.trim();
    clearTimeout(debounceTimer);

    if (q.length < 2) {
      hideResults();
      return;
    }

    debounceTimer = setTimeout(() => search(q), 300);
  });

  clearBtn.addEventListener("click", () => {
    input.value = "";
    hideResults();
    input.focus();
  });

  document.addEventListener("click", (e) => {
    if (!root.contains(e.target)) hideResults();
  });
})();
