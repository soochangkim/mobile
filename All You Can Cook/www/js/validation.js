/**
 * Created by skim7663 on 4/2/2017.
 */

// Validation Rules

function validate_frmRecipePost() {
    var form = $("#frmRecipePost");
    var ingredients = document.getElementsByName("txtIngredients[]");
    var quantity = document.getElementsByName("txtQuantity[]");

    form.validate({
        rules: {
            txtTitle: {
                required: true
            },
            txtCategory: {
                required: true
            },
            txtDirection: {
                required: true
            },
            txtWebsite: {
                rangelength: [4, 255],
                recipeUrl: true
            },
            txtName: {
                rangelength: [4, 32]
            },
            txtQuantity: {
                required: true
            },
            txtIngredients: {
                required: true
            }

        },
        messages: {
            txtTitle: {
                required: "Title is required"
            },
            txtCategory: {
                required: "Category is required"
            },
            txtDirection: {
                required: "Direction is required"
            },
            txtWebsite: {
                rangelength: "Address is too long",
                recipeUrl: "Please enter valid url address"
            },
            txtName: {
                rangelength: "Your name is too long or too short"
            },
            txtQuantity: {
                required: "Quantity is required"
            },
            txtIngredients: {
                required: "Ingredient is required"
            }
        }
    });

    var error =$("#error");
    if(isStringEmtpy($("#txtPostImage").val())){
        error.html("Please Upload Picture");
    }
    else {
        error.html("");
    }

    return form.valid() && isStringEmtpy(error.html());
}

jQuery.validator.addMethod("recipeUrl",
    function (val, ele, len) {
         var pattern = /^(www.)?[a-zA-Z]+.*\.[a-zA-Z]/;
        return this.optional(ele) || true;
    });