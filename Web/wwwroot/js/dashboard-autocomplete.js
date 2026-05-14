(function () {
  const MIN_CHARS = 2;
  const DEBOUNCE_MS = 250;

  const initAutocomplete = (root) => {
    const input = root.querySelector("[data-autocomplete-url]");
    const results = root.querySelector(".dashboard-search__results");

    if (!input || !results) {
      return;
    }

    let debounceId = null;

    const clearResults = () => {
      results.innerHTML = "";
      results.hidden = true;
    };

    const renderResults = (items) => {
      results.innerHTML = "";
      if (!items.length) {
        clearResults();
        return;
      }

      items.forEach((name) => {
        const option = document.createElement("button");
        option.type = "button";
        option.className = "dashboard-search__option";
        option.textContent = name;
        option.addEventListener("click", () => {
          input.value = name;
          clearResults();
          const listUrl = input.getAttribute("data-list-url");
          if (listUrl && window.dashboardLoadList) {
            const url = `${listUrl}?search=${encodeURIComponent(name)}`;
            window.dashboardLoadList(url);
          }
        });
        results.appendChild(option);
      });

      results.hidden = false;
    };

    const fetchResults = async () => {
      const term = input.value.trim();
      const listUrl = input.getAttribute("data-list-url");

      if (term.length < MIN_CHARS) {
        clearResults();
        if (term.length === 0 && listUrl && window.dashboardLoadList) {
          window.dashboardLoadList(listUrl);
        }
        return;
      }

      const url = input.getAttribute("data-autocomplete-url");
      if (!url) {
        return;
      }

      try {
        const response = await fetch(`${url}?term=${encodeURIComponent(term)}`);
        if (!response.ok) {
          clearResults();
          return;
        }

        const data = await response.json();
        const items = Array.isArray(data) ? data : [];
        renderResults(items);
      } catch {
        clearResults();
      }
    };

    input.addEventListener("input", () => {
      if (debounceId) {
        window.clearTimeout(debounceId);
      }
      debounceId = window.setTimeout(fetchResults, DEBOUNCE_MS);
    });

    document.addEventListener("click", (event) => {
      if (!root.contains(event.target)) {
        clearResults();
      }
    });
  };

  document
    .querySelectorAll("[data-autocomplete-root]")
    .forEach(initAutocomplete);

  window.dashboardInitAutocomplete = (root) => {
    root.querySelectorAll("[data-autocomplete-root]").forEach(initAutocomplete);
  };
})();
