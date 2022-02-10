// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//asp - area="user" asp - controller="cart"  asp - action="add" asp - route - id="@item.Id"

function AddToCart(id) {
    console.log(id)
    $.ajax({
        type: 'POST',
        url: '/Cart/Add/' + id,
       // contentType: 'application/x-www-form-urlencoded; charset=UTF-8', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
       // data: data,
        success: function (result) {
            alert(result["msg"]);
            document.getElementById("cartCount").innerHTML = result["count"];
            console.log(result);
        },
        error: function () {
            alert('You should Login First');
            console.log('Failed ');
        }
    })
}

