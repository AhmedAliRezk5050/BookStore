let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable(
        {
            "ajax": {
                "url": "/Admin/Product/GetAll"
            },
            "columns": [
                {"data": "title", "width": "15%"},
                {"data": "isbn", "width": "15%"},
                {"data": "price", "width": "15%"},
                {"data": "author", "width": "15%"},
                {"data": "category.name", "width": "15%"},
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                                 <div class="d-flex justify-content-center gap-2">
                                        <a href="/Admin/Product/Upsert/${data}" class="btn btn-warning d-inline-flex gap-1">
                                            <i class="bi bi-pencil-square"></i>Edit
                                        </a>
                                        <a  href="/Admin/Product/delete/${data}" class="btn btn-danger d-inline-flex gap-1">
                                           <i class="bi bi-trash"></i>Delete
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