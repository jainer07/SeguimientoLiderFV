(function () {
    const overlayId = "app-loading-overlay";

    function showLoading() {
        const el = document.getElementById(overlayId);
        if (!el) return;
        el.classList.remove("d-none");
        el.setAttribute("aria-hidden", "false");
    }

    function hideLoading() {
        const el = document.getElementById(overlayId);
        if (!el) return;
        el.classList.add("d-none");
        el.setAttribute("aria-hidden", "true");
    }

    // 1) Ocultar cuando el DOM ya está listo
    document.addEventListener("DOMContentLoaded", hideLoading);

    // 2) Mostrar cuando el usuario navega o recarga (links internos)
    document.addEventListener("click", function (e) {
        const a = e.target.closest("a");
        if (!a) return;

        // Ignora: anchors, new tab, downloads, js:void
        const href = a.getAttribute("href") || "";
        if (!href || href.startsWith("#") || href.startsWith("javascript:")) return;
        if (a.target === "_blank" || a.hasAttribute("download")) return;

        // Ignora links con data-no-loading
        if (a.dataset && a.dataset.noLoading === "true") return;

        showLoading();
    });

    // 3) Mostrar en submits (forms)
    document.addEventListener("submit", function (e) {
        const form = e.target;
        if (form && form.dataset && form.dataset.noLoading === "true") return;
        showLoading();
    });

    // 4) Exponer funciones por si quieres usarlas manualmente
    window.AppLoading = { show: showLoading, hide: hideLoading };
})();
