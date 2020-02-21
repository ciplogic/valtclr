import lexing.Lexer
import java.io.File
import java.lang.Exception

class Compiler
{

}

fun getFilesMatching (fullDirPath: File) : ArrayList<File> {
    val result = ArrayList<File>();
    var files = fullDirPath.listFiles()
    for(f in files) {
        if (f.isDirectory){
            var children = getFilesMatching(f)
            for(file in children) {
                result.add(file)
            }
        } else {
            result.add(f)
        }
    }
    return result
}

fun timeIt(message: String, action: ()->Unit){
    var start = System.currentTimeMillis()
    action()
    var end = System.currentTimeMillis()
    print("$message: ${end - start} ms")
}

fun main(args: Array<String>) {
    val fullFileName = "v-master/vlib/compiler/main.v"
    val fileIn = File(fullFileName);
    val lexer = Lexer()

    var tokens = lexer.tokenizeFile(fileIn);
    timeIt("Processing VLib") {
        val dirVal = File("v-master/vlib")
        var files = getFilesMatching(dirVal)
        for (f in files) {
            if (!f.name.endsWith(".v"))
                continue
            try {
                tokens = lexer.tokenizeFile(f);
            } catch (ex: Exception) {

            }


        }
    };

}