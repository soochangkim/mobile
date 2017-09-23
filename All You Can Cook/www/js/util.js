/**
 * Created by skim7663 on 4/2/2017.
 */

// Helper utilities

function successSqlExecution(){
    console.info("Success: SQL");
}

function errorSqlExecution(tx, error){
    console.info("Fatal Error: " + error.message);
}

function successTransaction(){
    console.info("Transaction succeed");
}

function errorTransaction(error){
    console.error(error.message);
}

function navigateNewPage(pageId){
    $(location).attr("href", "#"+pageId);
}
function doValidate_addItemForm() {

    var form = $("#addItemForm");
    form.validate({
        rules:{
            itemName:{
                required: true,
                minlength: 3,
                maxlength: 20
            },
            quantity: {
                required: true,
                range:[0,10]
            }
        },
        messages:{
            itemName:{
                required: "Item name is required",
                minlength: "Item name length must be at least 3-20 character long",
                maxlength:  "Item name length must not longer than 20 characters"
            },
            quantity:{
                required:"You must enter quantity",
                range:"Quantity must be 0-10"
            }
        }
    });
    return form.valid();
}

function updateIngredients(group, sibling, it){
    var parent = group.parent();
    if (group.get(0) === parent.children(":last-child").get(0)) {
        if($(it).val() != ""){
            parent.append("<div>" +
                "<label for='txtIngredients'>Ingredient</label>" +
                "<div class='ui-input-text ui-body-inherit ui-corner-all ui-shadow-inset'><input type='text' " +
                "name='txtIngredients' id='txtIngredients'/></div>" +
                "<label for='txtQuantity'>Quantity</label>" +
                "<div class='ui-input-text ui-body-inherit ui-corner-all ui-shadow-inset'><input " +
                "type='text' name='txtQuantity' id='txtQuantity'/></div></div>");
            var temp = parent.children(":last-child").children("div").children("input");
            $(temp[0]).on("keyup",txtIngredients_keyup);
            $(temp[1]).on("keyup", txtQuantity_keyup);
        }
    }
    else if($(it).val() == "" && $(sibling).val() == ""){
        group.remove();
    }
}

function isStringEmtpy(str){
    var result = false;
    if(str === null || str === "")
        result = true;
    return result;
}

function getIngredientsHTML(rows){
    var html = "";

    html +="<form id='frmIngredients'>"+
        "<fieldset data-role='controlgroup'>" +
        "<legend>Ingredients</legend>";
    for(var i =0 ; i < rows.length; i++){
        var row = rows.item(i);
        html +=
            "<input type='checkbox' name='txtDetailIngredients' id='ingredients"+i+"'" +
            "value='"+ row['id'] +"'/>"+
            "<label for='ingredients"+i+"'>"+row['quantity'] + row['name'] +"</label>";
    }
    html +=
        "</fieldset>" +
        "<input type='button' data-role='button' id='btnAddToShoppingList' data-theme='b'" +
        "name='btnAddToShoppingList' value='ADD TO SHOPPING LIST'/>" +
        "<input type='button' data-role='button' id='btnClear' style='background-color:yellow'" +
        "name='btnClear' value='CLEAR'/></form>";
    return html;
}

function getInstructionHTML(row){
    var html = "";
    html += "<h2>Instruction</h2><p>" + row['direction'] +"<br/>";

    if(row['source'] != null){
        html += "Source: " + row['source'];
    }
    html+="</p>";
    return html;
}

function btnAddToShoppingList_click(){
    addAllIngredientToShoppingList('ingredient');
    if(confirm("Ingrident Success fully added to shopping list\nDo you want to go to shopping list?"))
    {
        navigateNewPage("pgShoppingList");
    }
}

function addAllIngredientToShoppingList(id){
    var ingredients = $("#" + id + " input[type='checkbox']:checked").map(
        function(){
            return this.value;
        });
    var recipe_id = localStorage.getItem("recipe_id");
    for(var i = 0; i < ingredients.length; i++){
        insertShoppingListItme(recipe_id,  ingredients[i]);
    }
}

function clearSelectedIngredients(){
    var list = $('#frmIngredients').children("fieldset").children("div").children("input");
    ShoppingList.deleteByRecipeId([localStorage.getItem("recipe_id")]);
    list.prop("checked",false).checkboxradio("refresh");
}