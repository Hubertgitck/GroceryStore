var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    var urlParams = {
        "inprocess": "inprocess",
        "completed": "completed",
        "pending": "pending",
        "approved": "approved"
    };

    var param;
    for (var key in urlParams) {
        if (url.includes(key)) {
            param = urlParams[key];
            break;
        }
    }
    loadDataTable(param);
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?status=" + status
        },
        "language": {
            "emptyTable": "You did not order anything yet!"
        },
        "columns": [
            { "data": "id" , "width": "15%"},
            { "data": "name" , "width": "15%"},
            { "data": "phoneNumber" , "width": "15%"},
            { "data": "applicationUser.email" , "width": "15%"},
            { "data": "orderStatus" , "width": "15%"},
            { "data": "orderTotal" , "width": "15%"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="w-50 btn-group" role="group">
                        <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>
                            Details</a>
                    </div>`
                },
                "width": "15%"
            },
        ]
    });
}