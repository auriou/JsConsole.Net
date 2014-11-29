
'use strict';
(function () {
    String.prototype.isEmpty = function () {
        return (this.length === 0 || !this.trim());
    };
    var app = angular.module('app', ['SignalR']);
    app.factory('browserUtil', [
        '$rootScope', 'Hub', function ($rootScope, Hub) {
            var browserUtil = this;
            //declaring the hub connection
            var hub = new Hub('browserKit', {
                //client side methods
                listeners: {
                    'updateExec': function (data) {
                        browserUtil.script = data;
                        $rootScope.$apply();
                    },
                    'isConnected': function () {
                        browserUtil.connected = true;
                        $rootScope.$apply();
                    }
                },
                //server side methods
                methods: ['log','sendFunctions']
            });
            browserUtil.log = function (data) { hub.log(data.toString()); };
            browserUtil.sendFunctions = function (data) { hub.sendFunctions(data); };
            browserUtil.script = "";
            browserUtil.connected = false;

            return browserUtil;
        }
    ]);

    app.config(function ($provide) {
        $provide.decorator("$exceptionHandler", ['$delegate', function ($delegate) {
            return function (exception, cause) {
                $delegate(exception, cause);
                jsConsole.log("{{Error: " + exception.stack + "}}");
            };
        }]);
    });

    app.controller("mainController", [
        '$scope', 'browserUtil', function ($scope, browserUtil) {
            $scope.logs = [];
            $scope.browserUtil = browserUtil;

            $scope.execFunction = function(strFunction) {
                eval(strFunction);
            }

            $scope.retreiveFunctions = function (objInspect, nameObject) {
                if (objInspect === undefined)
                    return "";
                var strObj = "";
                if (nameObject !== undefined)
                    strObj = nameObject + ".";
                var result = "";
                for (var obj in objInspect) {
                    if (typeof objInspect[obj] === 'function') {
                        var fnStr = eval(strObj + obj + ".toString()");
                        fnStr = strObj + obj + "(" + fnStr.slice(fnStr.indexOf('(') + 1, fnStr.indexOf(')')) + ")";
                        result += fnStr + ";";
                    }
                }
                return result;
            }
            $scope.$watch('browserUtil.script', function (value) {
                if (!value.isEmpty()) {
                    eval(value);
                }
            });
            $scope.$watch('browserUtil.connected', function(value) {
                if (value === true) {
                    $scope.browserUtil.log("{{Connected:" + navigator.userAgent + "}}");
                    var result = $scope.retreiveFunctions(window);
                    if (jsConsole !== undefined)
                        result += $scope.retreiveFunctions(jsConsole, "jsConsole");
                    $scope.browserUtil.sendFunctions(result);
                }
            });
        }
    ]);
})();

(function() {
    var jsConsole = function() {
    }
    jsConsole.prototype.log = function(data) {
        angular.element(document.body).scope().browserUtil.log(data);
    }
    jsConsole.prototype.logAdd = function (texte) {
        angular.element(document.body).scope().logs.push(texte);
    }
    window.jsConsole = jsConsole;
})();

var jsConsole = new window.jsConsole();

