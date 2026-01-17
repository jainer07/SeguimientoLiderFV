window.addEventListener('DOMContentLoaded', () => {
    const langES = {
        placeholder: "Buscar...",
        searchTitle: "Buscar dentro de la tabla",
        perPage: "registros por página",
        noRows: "No hay registros para mostrar",
        info: "Mostrando {start} a {end} de {rows} registros",
        noResults: "No se encontraron resultados",
        loading: "Cargando...",
        infoFiltered: "(filtrado de {rowsTotal} registros)",
        previous: "Anterior",
        next: "Siguiente",
        first: "Primero",
        last: "Último"
    };

    function initTooltips(root = document) {
        root.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
            bootstrap.Tooltip.getOrCreateInstance(el, { container: 'body' });
        });
    }

    function setupTable(table) {
        if (table.dataset.dtReady === '1') return;

        const customConfig = table.dataset.dt ? JSON.parse(table.dataset.dt) : {};

        const dt = new simpleDatatables.DataTable(table, {
            labels: langES,
            ...customConfig
        });

        table.dataset.dtReady = '1';

        const refreshTooltips = () => initTooltips(document);
        refreshTooltips();

        dt.on('datatable.page', refreshTooltips);
        dt.on('datatable.sort', refreshTooltips);
        dt.on('datatable.search', refreshTooltips);
    }

    document.querySelectorAll('table[data-datatable]').forEach(setupTable);
});
