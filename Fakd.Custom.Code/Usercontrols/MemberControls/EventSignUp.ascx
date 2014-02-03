<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventSignUp.ascx.cs" Inherits="Ontranet.EventSignUp" %>

<div class="container" ng-app="myApp">
        <div class="row" ng-controller="stageController">
           
                <form name="myForm" novalidate ng-submit='submitForm()'>
                    <input type="hidden" name="eventName" ng-model="eventName" value="navent" copy-value />
                    <input type="hidden" name="eventDate" ng-model="eventDate" value="12/12/2014" copy-value />
                    <div class="form-group">
                        <input type="text" name="memberNumber" ng-model="memberNumber" placeholder="Dit medlemsnummer" class="form-control" />
                    </div>
                    <div class="form-group">
                        <input type="text" name="userName" ng-model="userName" required placeholder="Dit navn" class="form-control" />
                        <div class="custom-error" ng-show="myForm.userName.$dirty && myForm.userName.$invalid">
                            <span class="help-block" ng-show="myForm.userName.$error.required">Dit navn skal udfyldes</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <input type="tel" name="userMobil" ng-model="userMobil" required placeholder="Dit telefonnummer" class="form-control" ng-pattern="/^((\(?\+45\)?)?)(\s?\d{2}\s?\d{2}\s?\d{2}\s?\d{2})$/" />
                        <div class="custom-error" ng-show="myForm.userMobil.$dirty && myForm.userMobil.$invalid">
                            <span class="help-block" ng-show="myForm.userMobil.$error.required">Dit telefonnummer  skal udfyldes</span>
                            <span class="help-block" ng-show="myForm.userMobil.$error.pattern">Dit telefonnummer der ikke korrekt udfyldt i et af disse formater
                                <br />
                                (+45) 35 35 35 35 | +45 35 35 35 35 | 35 35 35 35 | 35353535</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <input type="email" name="userEmail" ng-model="userEmail" required ng-pattern="/^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/" placeholder="Din email" class="form-control" />
                        <div class="custom-error" ng-show="myForm.userEmail.$dirty && myForm.userEmail.$invalid">
                            <span class="help-block" ng-show="myForm.userEmail.$error.required">Email skal udfyldes</span>
                            <span class="help-block" ng-show="myForm.userEmail.$error.pattern">Email der ikke korrekt udfyldt</span>
                        </div>
                    </div>
                    <div class="form-group">
                         <div class="form-group">
                            <input type="submit" ng-disabled="myForm.userEmail.$pristine || myForm.userEmail.$dirty && myForm.userEmail.$invalid" class="btn btn-primary pull-right" value="Tilmeld" title="Tilmeld" />
                        </div>
                    </div>
                </form>
            
        </div>
    </div>

    <script data-require="angular.js@1.0.8" ata-semver="1.0.8" src="https://ajax.googleapis.com/ajax/libs/angularjs/1.0.8/angular.min.js"></script>

    <script src="/scripts/fakd/angularjs-init-script.js"></script>

    <style>
        .form-control.ng-invalid, .form-control.ng-invalid-required {
            border: 1px solid Red;
        }

        .form-control.ng-valid, .form-control.ng-pristine {
            border: 1px solid Green;
        }

        .help-block {
            font-size: smaller;
        }
        .form-control {
        border-bottom-color: #b3b3b3;
    border-bottom-left-radius: 3px;
    border-bottom-right-radius: 3px;
    border-bottom-style: solid;
    border-bottom-width: 1px;
    border-left-color: #b3b3b3;
    border-left-style: solid;
    border-left-width: 1px;
    border-right-color: #b3b3b3;
    border-right-style: solid;
    border-right-width: 1px;
    border-top-color: #b3b3b3;
    border-top-left-radius: 3px;
    border-top-right-radius: 3px;
    border-top-style: solid;
    border-top-width: 1px;
        }



    </style>