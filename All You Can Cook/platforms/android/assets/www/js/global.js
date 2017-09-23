/**
 * Created by skim7663 on 4/2/2017.
 */

// Global classes
var cameraReady = false;

function document_deviceready() {
    cameraReady = true;
}

function btnPostImage_click() {
    if (cameraReady) {
        getPhotoFromLibrary();
    } else {
        alert("Camera is not ready");
    }
}

function initDB() {
    try {
        DB.createDatabase();
        if (db != null) {
            DB.createTables();
        }
    }
    catch (e) {
        cosole.error("Fail: " + e.message);
    }
}
function pgShoppingList_show() {
    $("#lblRecipe").text("Spring roll");
    getShoppingList();

}
function btnaddItem_click() {
    addNewItemInShoppingList();
}
function btnclearItems_click() {
    clearShoppingList();
}

function pgGorceryMap_pagebeforeshow() {
    getPlacesLocation();
}

function pgShowRecipes_pagebeforeshow() {
    showAllRecipes();
}

function btnCancel_click() {
    navigateNewPage("pgShowRecipes");
}

function btnSubmit_click() {
    postRecipe();
}

function btnRecipePost_click() {
    navigateNewPage("pgPostRecipe");
}

function txtQuantity_keyup() {
    var group = $(this).parent().parent();
    var sibling = group.children("div").children("input")[0];
    updateIngredients(group, sibling, this);
}

function txtIngredients_keyup() {
    var group = $(this).parent().parent();
    var sibling = group.children("div").children("input")[1];
    updateIngredients(group, sibling, this);
}

function pgRecipeDetail_pagebeforeshow() {
    showRecipeDetails();
}
function deleteContact_click() {

    var options = new ContactFindOptions();
    options.filter = "Test User";
    options.multiple = false;
    fields = ["displayName"];

    navigator.contacts.find(fields, contactfindSuccess, contactfindError, options);

    function contactfindSuccess(contacts) {

        var contact = contacts[0];
        contact.remove(contactRemoveSuccess, contactRemoveError);

        function contactRemoveSuccess(contact) {
            alert("Contact Deleted");
        }

        function contactRemoveError(message) {
            alert('Failed because: ' + message);
        }
    }

    function contactfindError(message) {
        alert('Failed because: ' + message);
    }

}

function createContact_click() {
    createContact();
}

function findContact_click() {
    var options = new ContactFindOptions();
    options.filter = "";
    options.multiple = true;

    fields = ["displayName"];
    navigator.contacts.find(fields, contactfindSuccess, contactfindError, options);

    function contactfindSuccess(contacts) {
        for (var i = 0; i < contacts.length; i++) {
            alert("Display Name = " + contacts[i].displayName);
        }
    }

    function contactfindError(message) {
        alert('Failed because: ' + message);
    }

}

function init() {
    $(document).on("deviceready", document_deviceready);
    $("#pgShoppingList").on("pageshow", pgShoppingList_show);
    $("#btnaddItem").on("click", btnaddItem_click);
    $("#btnclearItems").on("click", btnclearItems_click);
    $("#btnRecipePost").on("click", btnRecipePost_click);
    $("#btnSubmit").on("click", btnSubmit_click);
    $("#btnCancel").on("click", btnCancel_click);
    $("#pgShowRecipes").on("pagebeforeshow", pgShowRecipes_pagebeforeshow);
    $("#txtQuantity").on("keyup", txtQuantity_keyup);
    $("#txtIngredients").on("keyup", txtIngredients_keyup);
    $("#pgRecipeDetail").on("pagebeforeshow", pgRecipeDetail_pagebeforeshow);
    $("#pgGroceryMap").on("pagebeforeshow", pgGorceryMap_pagebeforeshow);
    $("#createContact").on("click", createContact_click);
    $("#findContact").on("click", findContact_click);
    $("#deleteContact").on("click", deleteContact_click);
    $("#btnPostImage").on("click", btnPostImage_click);

}

$(document).ready(function () {
    initDB();
    init();
});