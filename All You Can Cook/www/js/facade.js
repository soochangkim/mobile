/**
 * Created by skim7663 on 4/2/2017.
 */

// Temporary examples
function clearShoppingList() {
    ShoppingList.delete();
    $(location).prop('href', "#pgMain");

}
function createContact() {
    var name= $("#txtContact").val();
    var myContact = navigator.contacts.create({"displayName": name});
    myContact.save(contactSuccess, contactError);
    $("#txtContact").val("");
    function contactSuccess() {
        alert("Contact is saved!")
    }

    function contactError(message) {
        alert('Failed because: ' + message);
    }

}
function addNewItemInShoppingList() {
    //check validation
    if (doValidate_addItemForm()) {
        console.info("Validation ok");
        //if validation is successful then fetch info from input controls(textbox etc)
        var itemName = $("#itemName").val();
        var quantity = $("#quantity").val();
        var recipe_id = "";
        var options = [itemName, quantity, recipe_id];
        localStorage.setItem("ingredient_name", itemName);
        Ingredient.insert(options);
        console.log("info added in ingredient table");
        getIngredientId();

    }
    else {
        console.info("Validation failed");
    }
}
function getIngredientId() {
    var ingredient_name = localStorage.getItem("ingredient_name");
    var options = [ingredient_name];

    function successSelectOne(tx, results) {
        var row = results.rows.item(0);
        var ingredient_id = row['id'];
        insertInShopping(ingredient_id);
    }

    Ingredient.findItem(options, successSelectOne);
}

function insertInShopping(ingredient_id) {
    var recipe_id = "";
    var options = [recipe_id, ingredient_id];
    ShoppingList.insert(options);
    $("#itemName").val("");
    $("#quantity").val("");

    $(location).prop('href', "#pgShoppingList");
}

function getShoppingList() {
    function successHandler(tx, results) {
        var htmlCode = "";
        var count = 0;
        var myArray = [];
        for (var i = 0; i < results.rows.length; i++) {
            var row = results.rows.item(i);
            if (row['title'] == null) {
                if (count == 0) {
                    htmlCode += "<h4 style='color: #a52a2a'>**My List**</h4><li data-icon='minus'><a data-row-id=" +
                        row['name'] + ">" + row["name"] + "</a></li>";
                }
                else {

                    htmlCode += "<li data-icon='minus'><a data-row-id=" +
                        row['name'] + ">" + row["name"] + "</a></li>";
                }
                count++;
            }
            else {
                myArray.push(row['title']);
                if (htmlCode.indexOf(row['title']) > -1) {
                    htmlCode += "<li data-icon='minus'><a data-row-id=" +
                        row['name'] + ">" + row["name"] + "</a></li>";

                } else {

                    htmlCode += "<h4 style='color: brown'>**" + row['title'] + "**</h4>" +
                        "<li data-icon='minus'><a data-row-id=" + row['name'] + ">" + row["name"] + "</a></li>";
                }
            }
        }
        var lv = $("#shoppingList");
        lv = lv.html(htmlCode);
        lv.listview("refresh");
        $("#shoppingList li").on("click", clickHandler);
        function clickHandler() {
            var style = $(this).css('text-decoration');
            console.log(style);
            if (style == 'none solid rgb(51, 51, 51)' || style == 'none solid rgb(0, 85, 153)') {
                console.log("inside if");
                $(this).removeClass();
                $(this).addClass("selectedItem");
            }
            else {
                $(this).removeClass();
                $(this).addClass("notselectedItem");
                console.log("inside else");
            }
        }
    }

    ShoppingList.selectAll(successHandler);
}

function postRecipe() {
    if (validate_frmRecipePost()) {
        Recipe.insert([
            $("#txtTitle").val(),
            $("#txtCategory").val(),
            $("#txtDirection").val(),
            $("#txtPostImage").val(),
            $("#txtName").val(),
            $("#txtWebsite").val()
        ], function (tx, results) {
            Ingredient.insertAll(results.insertId,
                $("input[name^='txtIngredients']"),
                $("input[name^='txtQuantity']"));
        });
            $("#txtTitle").val("");
            $("#txtCategory").val("");
            $("#txtDirection").val("");
            $("#txtPostImage").val("");
            $("#txtName").val("");
            $("#txtWebsite").val("");
        navigateNewPage("pgShowRecipes");
    }
}

function showAllRecipes() {
    function successHandler(tx, results) {
        if (results.rows.length != 0) {
            var list = $("#lstRecipe");
            var html = "";

            for (var i = 0; i < results.rows.length; i++) {
                var row = results.rows.item(i);
                html += "<li>" +
                    "<a data-row-id=" + row['id'] + "><img src='" + row['img'] + "'/>" + row["title"] + "</a></li>"
            }

            list.html(html).listview("refresh");
            list.children("li").children("a").on("click", function () {
                localStorage.setItem("recipe_id", this.getAttribute("data-row-id"));
                navigateNewPage("pgRecipeDetail");
            });
        }
    }

    Recipe.selectAll(successHandler);
}

function showRecipeDetails() {
    var id = localStorage.getItem("recipe_id");

    function successInstruction(tx, results) {

        var item =results.rows.item(0);
        if (item == null) {
            alert("Can not find recipe");
            navigateNewPage("pgShowRecipes");
        }
        $("#instruction").html(getInstructionHTML(item));
        $("#titleRecipeDetail").html(item["title"]);
        $("#imgRecipe").prop("src", item["img"]);
    }

    Recipe.select([id], successInstruction);

    function successIngredients(tx, results) {
        $("#ingredient").html(getIngredientsHTML(results.rows));
        $("#btnAddToShoppingList").on("click", btnAddToShoppingList_click);
        $("#btnClear").on("click", clearSelectedIngredients);
        $("input[type=checkbox]").checkboxradio().checkboxradio("refresh");
        $("input[type=button]").button().button("refresh");
    }

    Ingredient.selectByRecipeId([id], successIngredients);

    function sucessShoppingList(tx, results) {
        var len = results.rows.length;
        var ingridients = $('#frmIngredients').children("fieldset").children("div").children("input");
        for (var i = 0; i < len; i++) {
            var row = results.rows.item(i);
            var lenIngridients = ingridients.length;
            for (var j = 0; j < lenIngridients; j++) {
                var ingridient = $(ingridients[j]);
                if (row['recipe_id'] == id && row['ingredient_id'] == ingridient.val()) {
                    ingridient.prop("checked", true).checkboxradio("refresh");
                }
            }
        }
    }

    ShoppingList.selectAll(sucessShoppingList);
}

function insertShoppingListItme(recipeId, ingredientId) {
    function success(tx, results) {
        var len = results.rows.length;
        var isExsits = false;
        for (var i = 0; i < len; i++) {
            var row = results.rows.item(i);
            if (row['recipe_id'] == recipeId && row['ingredient_id'] == ingredientId) {
                isExsits = true;
            }
        }
        if (!isExsits)
            ShoppingList.insert([recipeId, ingredientId]);
    }

    ShoppingList.selectAll(success);
}