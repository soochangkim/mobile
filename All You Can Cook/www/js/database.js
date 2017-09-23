/**
 * Created by skim7663 on 4/2/2017.
 */

function error(e) {
    console.error("Fail to transaction:\n" + e.message);
}
function success() {
    console.info("Success transaction");
}

function errorHandler(tx, error) {
    console.error("SQL Error: " + tx + " (" + error.code + ") -- " + error.message);
}
function successTransaction() {
    console.info("Success: Transaction is successful");
}

var db;
var DB = {
    createDatabase: function () {
        var name = "AllYouCanCookDB";
        var version = "1.0";
        var displayName = "Database for Recipe App";
        var size = 2 * 2 * 1024;

        function success() {
            console.log("DB created");
        }

        db = openDatabase(name, version, displayName, size, success);
    },
    createTables: function () {
        function txFunction(tx) {
            function successHandler() {
                console.log("Success transaction");
            }

            function errorHandler(tx, e) {
                console.error("Error transaction");
                console.error(e.message);
            }



            var options = [];
            var sql = "CREATE TABLE IF NOT EXISTS recipe(" +
                "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "title VARCHAR(32) NOT NULL," +
                "category VARCHAR(32) NOT NULL," +
                "direction VARCHAR(1024) NOT NULL ," +
                "img VARCHAR(255) NOT NULL, " +
                "name VARCHAR(32)," +
                "website VARCHAR(255));";

            tx.executeSql(sql, options, successHandler, errorHandler);

            sql =
                "CREATE TABLE IF NOT EXISTS ingredient( " +
                "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "name VARCHAR(32) NOT NULL, " +
                "quantity VARCHAR(32) NOT NULL, " +
                "recipe_id INTEGER, " +
                "FOREIGN KEY(recipe_id) REFERENCES recipe(id));";

            tx.executeSql(sql, options, successHandler, errorHandler);

            sql =
                "CREATE TABLE IF NOT EXISTS shopping_list(" +
                "shopping_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "recipe_id INTEGER," +
                "ingredient_id INTEGER NOT NULL," +
                "FOREIGN KEY(recipe_id) REFERENCES recipe(id)," +
                "FOREIGN KEY(ingredient_id) REFERENCES ingredient(id));";

            tx.executeSql(sql, options, successHandler, errorHandler);
        }

        db.transaction(txFunction, error, success);
    }
};