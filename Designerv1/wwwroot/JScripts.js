function LoadTree(dotNetObject) {

    $('#tree_container').jstree({
        "plugins": ["wholerow", "search", "dnd", "types", "contextmenu"],
        'contextmenu': {
            'select_node': false,
            'items': function (node) {
                // access node as: node.id);
                // build your menu depending on node id
                var this_item_id = node.li_attr.id;
                return {
                    createItem: {
                        "label": "Open Objects in GridView",
                        "action": function (obj) {
                            
                        },
                        "_class": "class"
                    },
                    renameItem: {
                        "label": "Create New Instance",
                        "action": function (obj) {
                            
                            dotNetObject.invokeMethodAsync('NavigateToCreate', this_item_id);
                        }
                    },
                    deleteItem: {
                        "label": "Add to Clipboard",
                        "action": function (obj) { this.remove(obj); }
                    },
                    "addndo": {
                        "label": "Add to Clipboard And",
                        "action": function (obj) { this.remove(obj); }
                    }
                };
            }
        }
    })

        //contextmenu: {items: CustomMenu}

    
        .bind("dblclick.jstree", function (event) {
            var node = $(event.target).closest("li");
            /*var data = node.data("jstree");*/
            dotNetObject.invokeMethodAsync('asdasd', node.attr('id'));
            //alert(node.attr('id'));
            });
}

/*
 hay basicamente 2 metodos para llamar metodos c# desde js.
            DotNet.invokeMethodAsync('Designerv1', 'asd');
            dotNetObject.invokeMethodAsync( 'asdasd');


    */


function csharpcall() {


}



//https://localhost:5001/api/ObjecTree