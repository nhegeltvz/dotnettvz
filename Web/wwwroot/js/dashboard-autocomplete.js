// IIFE, funkcija se pokreće cim se skripta pokrene
(function () {
  const DEBOUNCE_MS = 250;

  function debounce(fn, ms) {
    let timer;
    return (...args) => {
      clearTimeout(timer);
      timer = setTimeout(() => fn(...args), ms);
    };
  }

  // init sets up one autocomplete instance for a given root element (the [data-autocomplete-root] div).
  // It reads the config from data-* attributes on the input, then wires up all the event listeners.
  // By receiving `root` as a parameter, this same function can run independently on multiple inputs on the page.
  function init(root) {
    const input = root.querySelector("input[data-autocomplete-url]");
    const box = root.querySelector(".dashboard-search__results");
    if (!input || !box) return;

    // These come from data-* attributes set by the Razor partial (_AutocompleteSearch.cshtml).
    // searchUrl  -> where to fetch suggestions from (e.g. /players/suggestions)
    // labelField -> which property of the returned JSON object to show as text (e.g. "username")
    const searchUrl = input.dataset.autocompleteUrl;
    const labelField = input.dataset.labelField || "name";

    // Takes the array of objects returned by the server and builds a div for each one inside the dropdown box.
    // item[labelField] reads a dynamic property name from the object, e.g. item["username"] -> "Ronaldo".
    // The ?? fallback handles the edge case where labelField doesn't match any property.
    function renderSuggestions(items) {
      box.innerHTML = "";
      if (!items.length) {
        box.hidden = true;
        return;
      }

      items.forEach((item) => {
        const opt = document.createElement("div");
        opt.className = "dashboard-search__option";
        opt.textContent = item[labelField] ?? String(Object.values(item)[0]);
        // mousedown fires before the input's blur event, so the dropdown is still open when this runs.
        // If we used 'click' instead, the input would blur first, closeBox() would run, and the click would never land.
        opt.addEventListener("mousedown", (e) => {
          e.preventDefault(); // prevents the input from losing focus at all
          input.value = opt.textContent;
          closeBox();
          dispatchConfirm(opt.textContent);
        });
        box.appendChild(opt);
      });
      box.hidden = false;
    }

    // Hides the dropdown and clears its contents.
    function closeBox() {
      box.hidden = true;
      box.innerHTML = "";
    }

    // Fires a custom browser event called 'autocomplete:confirm' on the root element.
    // CustomEvent lets us attach data (the search term) via the `detail` property.
    // `bubbles: true` means the event travels up the DOM, so a parent element can also catch it if needed.
    // The page-specific JS (e.g. players.js) listens for this event and decides what to do — load the table, etc.
    // This is what keeps the autocomplete generic: it doesn't know or care what happens after confirm.
    function dispatchConfirm(term) {
      root.dispatchEvent(
        new CustomEvent("autocomplete:confirm", {
          bubbles: true,
          detail: { term },
        }),
      );
    }

    const fetchSuggestions = debounce(async (term) => {
      if (!term.trim()) {
        closeBox();
        return;
      }
      try {
        const res = await fetch(
          `${searchUrl}?search=${encodeURIComponent(term)}`,
        );
        renderSuggestions(await res.json());
      } catch {
        closeBox();
      }
    }, DEBOUNCE_MS);

    // Every time the input value changes (any keystroke), kick off a debounced suggestions fetch.
    input.addEventListener("input", () => fetchSuggestions(input.value));

    // Enter confirms the current text in the input (same as clicking a suggestion).
    // Escape clears everything and reloads the full list (empty search = no filter).
    input.addEventListener("keydown", (e) => {
      if (e.key === "Enter") {
        e.preventDefault(); // stops the browser from submitting a form if the input is inside one
        closeBox();
        dispatchConfirm(input.value);
      }
      if (e.key === "Escape") {
        closeBox();
        input.value = "";
        dispatchConfirm("");
      }
    });

    //clear the search
    const clearBtn = root.querySelector(".dashboard-search__clear");
    if (clearBtn) {
      clearBtn.addEventListener("click", () => {
        closeBox();
        input.value = "";
        dispatchConfirm("");
      });
    }

    // close suggestions box when clicked outside of the input.
    document.addEventListener("click", (e) => {
      if (!root.contains(e.target)) closeBox();
    });
  }
  // initialize every autocomplete input.
  document.querySelectorAll("[data-autocomplete-root]").forEach(init);
})();
