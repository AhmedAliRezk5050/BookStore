let dataTable;

$(document).ready(function () {
    $(".filter-btn").click(function() {
        const status = $(this).data("status");
        const btn = $(this);
        $(".filter-btn")
        btn.siblings().removeClass("active");
        btn.addClass("active");
        $('#tblData').DataTable().search(status === "all" ? "" : status).draw();
    });
    
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable(
        {
            "ajax": {
                "url": "/Admin/Order/GetAll",
                // dataSrc because the response is an array
                dataSrc:""
            },
            "columns": [
                { "data": "id", "width": "5%"},
                { "data": "name", "width": "20%"},
                { "data": "phoneNumber", "width": "15%"},
                { "data": "applicationUser.email", "width": "20%"},
                { "data": "orderStatus", "width": "15%"},
                { "data": "orderTotal", "width": "15%"},
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                                 <div class="d-flex justify-content-center gap-2">
                                        <a href="/Admin/Order/Details/${data}" class="btn btn-warning d-inline-flex gap-1">
                                            <i class="bi bi-card-text"></i>
                                        </a>
                                 </div>  
                        `;
                    },
                    "width": "5%"
                }
            ]
        }
    );
}

