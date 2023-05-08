var siteUrl =  '';

var parsleys = {

    configError: {

        errorClass: 'has-error-border',

        successClass: 'has-success',

        errorsMessagesDisabled: true

    }

};

$(function() {

   siteUrl = $('meta[name=siteUrl]').attr("content");   

   if ($('.header-with-slick .slick-dots li').length<=1){

        $('.header-with-slick .slick-dots li').hide();

   }

   WebApp.init();

});



var WebApp = {

	init: function(){

		WebApp.Brand.init();

        WebApp.About.init();

        WebApp.ContactUs.init();

        WebApp.Newsletter.init();

        WebApp.Service.init();

        WebApp.Customer.init();

        WebApp.News.init();

        $('.js-btn-search').on("click",WebApp.search);

        $('.js-txtsearch-key').on("keypress" ,WebApp.enterSearch);



	},

    openModalBox: function(evt){

        evt.preventDefault();

        var btn = $(this);

        var param = {};

        var urlRequest = $(this).attr('href');

        serverCall.callFunc(urlRequest, function (response) {

          if (response!='') {

            $('.modal-brand-detail').html(response);

            $('.modal-brand-detail').modal('show');

          }

        }, "GET", "html",  param);

    },

    search:function(s)

    {

        

        var $btn = $(this);   

        var $inputField = $('.js-txtsearch-key');

        var strKey = $inputField.val().replace(/<\/?[^>]+(>|$)/g, "");

        if (validates.isEmpty(strKey) || strKey.length < 2){

            $btn.focus();

            return false;

        }

        var requestUrl = $inputField.attr('data-url');

        window.location.href = requestUrl+''+encodeURI('?keyword='+strKey);

        return false;

    },

    enterSearch : function(e) {

        if(e.which == 13) 

        {

           var $inputField = $(this);

           var strKey = $inputField.val().replace(/<\/?[^>]+(>|$)/g, "");

           if(validates.isEmpty(strKey)){

                showmsg.error($inputField.attr('msg-require'));

                return;

           }

           if(strKey.length <2){

                showmsg.error($inputField.attr('msg-valid'));

                 return;

           }

           else

           {

                var requestUrl = $inputField.attr('data-url');

                window.location.href = requestUrl+''+encodeURI('?keyword='+strKey);

            }

            $inputField.blur();

            return false;

        }

    },



}

WebApp.Customer =  {

    init: function(){        

        // $('.js-btn-register-customer').on('click', WebApp.Customer.submitRegisterCustomer);

    },

    submitRegisterCustomer: function(){

        var $form = $('#frm-registercustomer-form');

        var $buttonSubmit = $(this);               

        if ($form.parsley(parsleys.configError).validate())

        {   

            $txtPhone = $('#register_form_phone');

            if (!validates.isPhone($txtPhone.val())){

                $txtPhone.focus();

                $txtPhone.removeClass('has-success');

                $txtPhone.addClass('has-error');

                return false;

            }

            var isOther = $('input:radio[name="register_form[nationality]"]').filter(":checked").val();

            var $otherCountry = $('#txtOtherCountry');

            if (isOther=='Other' && validates.isEmpty($otherCountry.val()))

            {

                $otherCountry.focus();

                $otherCountry.addClass('has-error');

                return false;

            }

            var param = $form.serializeArray();              

            var urlRequest = $buttonSubmit.attr('data-url');

            serverCall.callFunc(urlRequest, function (response) {              

              if (response.status==1) {

                $('#frm-registercustomer-form').trigger("reset");

                if (response.url!=''){

                    window.location.href = response.url;

                    return;

                }

                showmsg.success(response.msg);

              }

              else{

                $('input[name="_token"]').val(response.token);

                showmsg.error(response.msg);

              }



            }, "POST", "json",  param);



        }

        return false;

    }

}

WebApp.News =  {

    init: function(){        

        $('.js-news-filter').on('change', WebApp.News.blockFilter);

    },

    blockFilter: function(evt){       

        var cateId = $("select[name=category]").val();

        var time = $("select[name=time]").val();

        var brand = $("select[name=brandId]").val();

        var urlRequest = $('input[name=filterUrl]').val();

        var params = [];

        if (cateId > 0){

            params.push('cid='+cateId);

        }

        if (time!=''){

            params.push('time='+time);

        }

        if (brand>0){

            params.push('brand='+brand);

        }

        window.location.href = urlRequest+'?'+params.join('&');

    },

}



WebApp.Service =  {

    init: function(){        

        $('.js-service-loadmore').on('click', WebApp.Service.loadMore);

    },

    loadMore: function(evt){

        evt.preventDefault();

        var btn = $(this);

        var param = {};

        var urlRequest = $(this).attr('data-url');

        var container = $(this).attr('data-container');

        serverCall.callFunc(urlRequest, function (response) {            

          if (response.content!='') 

          {            

             $(container).append(response.content);

             if (!validates.isEmpty(response.url)){

                $(btn).attr('data-url',response.url);

             } 

             else{

                $(btn).remove();

             }            

              $('.js-open-model-box').on('click',WebApp.openModalBox);

          }          

        }, "GET", "json",  param);





    },

}



WebApp.Newsletter =  {

    init: function(){

        // $('.js-language-switcher').on('change', function(){

        //     window.location.href = $(this).val();

        // });

        // $('.js-signup-newsletter').on('click', WebApp.Newsletter.submitNewsletter);

    },

    // submitNewsletter: function(){

    //     var $form = $('#form-newsletter');

    //     var $buttonSubmit = $(this);

    //     if ($form.parsley(parsleys.configError).validate())

    //     {



    //         var param = {

    //             'doact': 'newsletter',

    //             'email': $form.find('input[type=email]').val(),

    //         };

    //         var urlRequest = $buttonSubmit.attr('data-url');

    //         serverCall.callFunc(urlRequest, function (response) {

    //           if (response.status==1) {

    //             $('#form-newsletter').trigger("reset");

    //             showmsg.success(response.msg);

    //           }

    //           else{

    //             showmsg.error(response.msg);

    //           }



    //         }, "POST", "json",  param);



    //     }

    //     return false;

    // }

}

WebApp.ContactUs =  {

    init: function(){

         // $('.js-button-contact').on('click', WebApp.ContactUs.submitContact);

    },

    // submitContact: function(){

    //     var $form = $('#form-contactus');

    //     var $buttonSubmit = $(this);

    //     if ($form.parsley(parsleys.configError).validate())

    //     {



    //         var param = {

    //             'doact': 'contact',

    //             'name': $('#txtContactName').val(),

    //             'email': $('#txtContactEmail').val(),

    //             'phone': $('#txtContactPhone').val(),

    //             'message': $('#txtContactMessage').val(),

    //             'partner': $('input[name=partner_type]:checked').val(),                

    //         };

    //         var urlRequest = $buttonSubmit.attr('data-url');

    //         serverCall.callFunc(urlRequest, function (response) {

    //           if (response.status==1) {

    //             $('#form-contactus').trigger("reset");

    //             showmsg.success(response.msg);



    //           }

    //           else{

    //             showmsg.error(response.msg);

    //           }



    //         }, "POST", "json",  param);



    //     }

    //     return false;

    // }

}

WebApp.About =  {

    init: function(){

       // $('.js-about-albumtab').on('click',WebApp.About.showAlbum);

    },

    showAlbum: function(){

        var target = $(this).attr('data-container');

        $(this).addClass('active');

        $('.js-album-content').hide();

        $(target).show();

    }

};

WebApp.Brand =  {

	init: function(){

		$('.js-filter-brandhome').on('change', WebApp.Brand.loadHome);

        $('.js-brand-tablist > li > a').on('click',WebApp.Brand.initTab);

        $('.js-filter-brand-aphabet').on('click',WebApp.Brand.loadDataAphabet);

        $('.js-filter-brand-floor').on('click',WebApp.Brand.loadDataAphabet);

        $('.js-open-model-box').on('click',WebApp.openModalBox);

        if (jsonBrands.length> 0 && $(".js-txtautosuggest-brand").length > 0)

        {            

            var options = {

                data: jsonBrands,

                getValue: "value",

                list: {

                    maxNumberOfElements: 10,

                    match: {

                        enabled: true

                    },

                    onKeyEnterEvent:  function() {

                         var suggestion = $(".js-txtautosuggest-brand").getSelectedItemData();

                         if (!validates.isEmpty(suggestion.url))

                            {

                               serverCall.callFunc(suggestion.url, function (response) {

                               if (response!='') {

                                    $('.modal-brand-detail').html(response);

                                    $('.modal-brand-detail').modal('show');

                               }

                               }, "GET", "html",  {}); 

                            } 

                        

                    },  

                    onClickEvent: function() {
                         var suggestion = $(".js-txtautosuggest-brand").getSelectedItemData();
                         if (!validates.isEmpty(suggestion.url)){
                            if(suggestion.type == 'link'){
                                window.open(suggestion.url);
                            }else{
                                serverCall.callFunc(suggestion.url, function (response) {
                                    if (response!='') {
                                        $('.modal-brand-detail').html(response);
                                        $('.modal-brand-detail').modal('show');
                                    }
                                }, "GET", "html",  {}); 
                            }
                        }
                    }  

                }

            };

            $(".js-txtautosuggest-brand").easyAutocomplete(options);

        }

	},    

    loadDataAphabet: function(evt){

        evt.preventDefault();

        var btn = $(this);

        var param = {};

        var urlRequest = $(this).attr('data-url');

        var container = $(this).attr('data-container');

        serverCall.callFunc(urlRequest, function (response) {

            $('.js-filter-brand-aphabet').removeClass('active');

            $('.js-filter-brand-floor').removeClass('active');

            $(btn).addClass('active');

          if (response!='') {

           $(container).html(response);

           $('.js-open-model-box').on('click',WebApp.openModalBox);

           window.page.brands.slickTab01Refresh(true);

          }

        }, "GET", "html",  param);





    },

    initTab: function(){

        var param = {};

        var tabname = $(this).attr('href');

        var urlRequest = $(this).attr('data-url');

        var container = $(this).attr('data-container');

        serverCall.callFunc(urlRequest, function (response) {

          if (response!='') {

           $(container).html(response);

           $('.js-open-model-box').on('click',WebApp.openModalBox);

           if (tabname=='#tab-01') {

                $('.js-filter-brand-aphabet').removeClass('active');

                $('.js-letter-all').addClass('active');

                window.page.brands.slickTab01Refresh(true);

           }

           else if (tabname=='#tab-02') {

                window.page.brands.slickTab02Refresh(true);

           }

           else{

                $('.js-filter-brand-floor').removeClass('active');

                $('.js-floor-first').addClass('active');

                window.page.brands.slickTab03Refresh(true);

           }

          }

        }, "GET", "html",  param);



    },

	loadHome: function(){

		var brandName = $(this).val();

		var param = {

			'brand': brandName

		};

		var urlRequest = $(this).attr('data-url');

        serverCall.callFunc(urlRequest, function (response) {

            if (response!='') {

               $('.js-brand-by-category').html(response);

               $('.js-open-model-box').on('click',WebApp.openModalBox);

               $("#brands-01 .slick").slick();

            }

        }, "GET", "html",  param);

	}

}



var serverCall = {

    requestUrl: "",

    isProcessing: false,

    callFunc: function (url, func, type, dataType, post) {

        if (!serverCall.isProcessing) {

            serverCall.isProcessing = true;

            if (type === undefined) {

                type = "GET";

            }

            if (post === undefined) {

                post = {};

            }

            if (dataType === undefined) {

                dataType = "text";

            }

            if (post._token === undefined || 0 === post._token.length) {

                post._token = $('input[name="_token"]').val();            

            }                                    

            $.ajax({

                type: type,

                url: serverCall.requestUrl + url,

                data: post,

                cache: false,

                dataType: dataType,

                headers: {

                    "is_ajax":true,

                    "language":"en"

                },

                success: function (response) {

                    serverCall.isProcessing = false;

                    if (typeof func === "function") {

                        func(response);

                    }

                },

                error: function (response) {

                    serverCall.isProcessing = false;

                }

            });

        } else {

            //showmsg.success("Please waiting...");

        }

    }

};



var showmsg = {

    alert: function (msg, callBack) {

        swal(msg).then((value) => {

         if (typeof callBack === "function") {

            callBack();

         }

        });



    },

    error: function (msg, callBack) {

        swal('', msg,'error').then((value) => {

         if (typeof callBack === "function") {

            callBack();

         }

        });

    },

    success: function (msg, callBack) {

        swal("", msg, "success");

    },

    warning: function (msg, callBack) {

        swal("", msg, "warning");

    }

};

var validates = {

    regExForEmail: /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,

    isUndefined: function (strObj) {

        return typeof strObj == 'undefined' ? true : false;

    },

    isEmpty: function (input) {

        return (!input || 0 === input.length);

    },

    isPhone: function (phone) {

        var flag = false;

        phone = phone.replace('(+84)', '0');

        phone = phone.replace('+84', '0');

        phone = phone.replace('0084', '0');

        phone = phone.replace(/ /g, '');

        if (phone != '') {

            var firstNumber = phone.substring(0, 2);

            if ((firstNumber == '09' || firstNumber == '08') || phone.length == 10) {

                if (phone.match(/^\d{10}/)) {

                    flag = true;

                }

            } else if (firstNumber == '01' && phone.length == 11) {

                if (phone.match(/^\d{11}/)) {

                    flag = true;

                }

            }

        }

        return flag;

    },

    isEmail: function (sEmail) {

        sEmail = sEmail.trim();

        if (sEmail.search(this.regExForEmail) != -1)

            return true;

        return false;

    },

    isIdno: function (idNo) {

        idNo = idNo.trim();

        if (isNaN(idNo) || idNo.length > 12 || idNo.length < 9)

            return false;

        return true;

    },

    parseInt: function (value) {

        value = parseInt(value);

        if (isNaN(value))

            return 0

        return value;

    },

}



