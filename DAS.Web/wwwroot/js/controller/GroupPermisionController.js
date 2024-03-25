var GroupPermisionConfig = {
    Init: function () {
        $("#ckIsBase").click(function () {
            if ($("#ckIsBase").is(":checked")) {
                $("#IsBase").val(true);
            } else {
                $("#IsBase").val(false);
            }
        });

        $("#BasePermissions").on("change", function () {
            if ($(this).val() > 0) {
                let idGroupPer = $(this).val();
                //get permission by idGroupPer
            }
        });
    },
    InitTree: function () {
        $.fn.extend({
            treed: function (o) {

                var openedClass = 'fa-plus';
                var closedClass = 'fa-minus';

                if (typeof o != 'undefined') {
                    if (typeof o.openedClass != 'undefined') {
                        openedClass = o.openedClass;
                    }
                    if (typeof o.closedClass != 'undefined') {
                        closedClass = o.closedClass;
                    }
                };
                //initialize each of the top levels
                var tree = $(this);
                tree.addClass("tree");
                tree.find('li').has("ul").each(function () {
                    var branch = $(this); //li with children ul
                    branch.prepend("<i class='fa " + closedClass + " tree-icon' ></i>");
                    branch.addClass('branch');
                    branch.on('click', function (e) {
                        if (this == e.target) {
                            var icon = $(this).children('i:first');
                            icon.toggleClass(openedClass + " " + closedClass);
                            $(this).find("ul").toggle();
                        }
                    })
                    branch.find(".tree-icon").on('click', function (e) {
                        if (this == e.target) {
                            jQuery(this).toggleClass(openedClass + " " + closedClass);
                            $(this).parent().find("ul").toggle();
                        }
                    })
                    //branch.children().children().toggle();
                });
                //fire event from the dynamically added icon
                tree.find('.branch .indicator').each(function () {
                    $(this).on('click', function () {
                        $(this).closest('li').click();
                    });
                });
                //fire event to open branch if the li contains an anchor instead of text
                tree.find('.branch>a').each(function () {
                    $(this).on('click', function (e) {
                        $(this).closest('li').click();
                        e.preventDefault();
                    });
                });
                //fire event to open branch if the li contains a button instead of text
                tree.find('.branch>button').each(function () {
                    $(this).on('click', function (e) {
                        $(this).closest('li').click();
                        e.preventDefault();
                    });
                });
            }
        });
        //Initialization of treeviews
        $('.useTree').each(function () {
            $(this).treed({ openedClass: 'fa-caret-right', closedClass: 'fa-caret-down' });
        });
    },
    CheckBoxTree: function () {
        $('.useTree').find('input[type=checkbox]').on('change', function () {
            //Thằng này là bố
            var parent = $(this).closest('ul').prev().find("input[type='checkbox']");
            //Thằng này là con
            var children = $(this).parent().next().find('input[type=checkbox]');
            //Những thằng anh em
            var sibling = $(this).closest('ul').find('input[type=checkbox]');
            //Những anh em đã lấy vợ
            var siblingChecked = $(this).closest('ul').find('input[type=checkbox]:checked');
            //Tất cả đã lấy vợ
            var isCheckAll = siblingChecked.length == sibling.length;
            //Chỉ một số lấy vợ
            var isCheckIndeterminate = (siblingChecked.length > 0) && (siblingChecked.length < sibling.length);

            // children checkboxes depend on current checkbox
            children.prop('checked', this.checked);
            // go up the hierarchy - and check/uncheck depending on number of children checked/unchecked
            parent.prop({ 'checked': isCheckAll, 'indeterminate': isCheckIndeterminate });
        });
    }
};
