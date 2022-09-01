let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable(
        {
            "ajax": {
                "url": "/Admin/Company/GetAll"
            },
            "columns": [
                { "data": "name", "width": "15%"},
                { "data": "streetAddress", "width": "15%"},
                { "data": "city", "width": "15%"},
                { "data": "state", "width": "15%"},
                { "data": "postalCode", "width": "15%"},
                { "data": "phoneNumber", "width": "15%"},
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                                 <div class="d-flex justify-content-center gap-2">
                                        <a href="/Admin/Company/Upsert/${data}" class="btn btn-warning d-inline-flex gap-1">
                                            <i class="bi bi-pencil-square"></i>Edit
                                        </a>
                                        <a  onclick="deleteCompany('/Admin/Company/delete/${data}')" class="btn btn-danger d-inline-flex gap-1">
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

function deleteCompany(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url,
                type: "DELETE",
                success: function(data) {
                    if(data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}

