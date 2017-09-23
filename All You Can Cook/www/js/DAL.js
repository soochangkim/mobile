/**
 * Created by skim7663 on 4/2/2017.
 */

var Recipe = {
    insert: function (options, successHandler) {
        function txFunction(tx) {
            var sql =
                "INSERT INTO recipe(title, category, direction, img, name, website)" +
                "VALUES(?,?,?,?,?,?);";
            tx.executeSql(sql, options, successHandler, errorSqlExecution);
        }

        db.transaction(txFunction, errorTransaction, successTransaction);
    },
    update: function (options) {
        function txFunction(tx) {
            var sql =
                "UPDATE recipe " +
                "SET title=?, category=?, direction=?, name=?, website=?" +
                "WHERE id=?";
            tx.executeSql(sql, options, successSqlExecution, errorSqlExecution);
        }

        db.transaction(txFunction, errorTransaction, successTransaction);
    },
    delete: function (options) {
        function txFunction(tx) {
            var sql = "DELETE FROM recipe WHERE id=?;";
            tx.executeSql(sql, options, successSqlExecution, errorSqlExecution);
        }

        db.transaction(txFunction, errorTransaction, successTransaction);
    },
    selectAll: function (successHandler) {
        function txFunction(tx) {
            var sql = "SELECT * FROM recipe;";
            tx.executeSql(sql, [], successHandler, errorSqlExecution);
        }

        db.transaction(txFunction, errorTransaction, successTransaction);
    },
    select: function (options, successHandler) {
        function txFunction(tx) {
            var sql = "SELECT * FROM recipe WHERE id=?";
            tx.executeSql(sql, options, successHandler, errorSqlExecution);
        }

        db.transaction(txFunction, errorTransaction, successTransaction);
    }
};

var ShoppingList = {
    insert: function (options) {
        function txFunction(tx) {
            var sql =
                "INSERT INTO shopping_list(recipe_id, ingredient_id) VALUES(?,?);";
            tx.executeSql(sql, options, successSqlExecution, errorSqlExecution);
        }

        console.info("added to shoping list");
        db.transaction(txFunction, errorHandler, successTransaction);
    },
    delete: function () {
        function txFunction(tx) {
            var sql = "DELETE FROM shopping_list;";
            tx.executeSql(sql, [], successSqlExecution, errorSqlExecution);
        }

        db.transaction(txFunction, errorHandler, successTransaction);
    },
    deleteByRecipeId: function(options){
        function txFunction(tx){
            var sql = "DELETE FROM shopping_list WHERE recipe_id = ?";
            tx.executeSql(sql, options, successSqlExecution, errorSqlExecution);
        }
        db.transaction(txFunction, errorHandler, successTransaction);
    },
    selectAll: function (successHandler) {
        function txFunction(tx) {
            var sql =
                "SELECT r.title AS title, i.name AS name, sl.recipe_id AS recipe_id, sl.ingredient_id AS ingredient_id " +
                "FROM shopping_list AS sl " +
                "JOIN ingredient AS i ON i.id = sl.ingredient_id " +
                "LEFT JOIN recipe AS r ON i.recipe_id = r.id; ";
            var options = [];
            tx.executeSql(sql, options, successHandler, errorSqlExecution);
        }

        db.transaction(txFunction, errorHandler, successTransaction);
    }
};

var Ingredient = {
    insert: function (options) {
        function txFunction(tx) {
            var sql =
                "INSERT INTO ingredient(name, quantity, recipe_id)" +
                "VALUES(?,?,?);";
            tx.executeSql(sql, options, successSqlExecution.errorSqlExecution);
        }

        db.transaction(txFunction, errorTransaction, successTransaction);
    },
    update: function (options) {
        function txFunction(tx) {
            var sql =
                "UPDATE ingredient " +
                "SET name=? " +
                "WHERE id=?;";
            tx.executeSql(sql, options, successSqlExecution, errorSqlExecution);
        }

        db.transaction(txFunction, errorTransaction, successTransaction);
    },
    insertAll: function (id, name, quantity) {
        for (var i = 0; i < name.length; i++) {
            if (!isStringEmtpy(name[i]['value']) && !isStringEmtpy(quantity[i]['value']))
                Ingredient.insert([name[i]['value'], quantity[i]['value'], id]);
        }
    },
    selectByRecipeId: function (options, successHandler) {
        function txFunction(tx) {
            var sql = "SELECT * FROM ingredient WHERE recipe_id = ?;";
            tx.executeSql(sql, options, successHandler, errorSqlExecution);
        }

        db.transaction(txFunction, errorTransaction, successTransaction);
    },
    findItem: function (options, successSelectOn) {
        function txFunction(tx) {
            var sql = "SELECT id FROM ingredient WHERE name=?;";
            tx.executeSql(sql, options, successSelectOn, errorHandler);
        }

        db.transaction(txFunction, errorHandler, successTransaction);
    }
};
