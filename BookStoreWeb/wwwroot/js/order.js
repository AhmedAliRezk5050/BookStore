let dataTable;

$(document).ready(function () {
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
                { "data": "id", "width": "15%"},
                { "data": "name", "width": "15%"},
                { "data": "phoneNumber", "width": "15%"},
                { "data": "applicationUser.email", "width": "15%"},
                { "data": "orderStatus", "width": "15%"},
                { "data": "orderTotal", "width": "15%"},
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                                 <div class="d-flex justify-content-center gap-2">
                                        <a href="/Admin/Order/Details/${data}" class="btn btn-success d-inline-flex gap-1">
                                            <i class="bi bi-pencil-square"></i>Details
                                        </a>
                                 </div>  
                        `;
                    },
                    "width": "15%"
                }
            ]
        }
    );
}

