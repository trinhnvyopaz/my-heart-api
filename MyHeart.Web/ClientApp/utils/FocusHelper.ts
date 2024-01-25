export default {
    focusEditor(editor){
        var text = editor.getText()
        if(text != null){                    
            editor.setSelection(text.length - 1) 
        }else{
            editor.focus()
        }
    },

    delay(ms: number) {
        return new Promise( resolve => setTimeout(resolve, ms) );
    }
}