package pex.app.main;

import java.io.IOException;
import java.io.FileNotFoundException;
import java.lang.ClassNotFoundException;
import java.lang.NullPointerException;

import pex.app.App;
import pex.core.loader.ProgramReaderText;
import pex.parser.BadSourceException;
import pex.parser.BadNumberException;
import pex.parser.InvalidExpressionException;
import pt.utl.ist.po.ui.Command;

/**
* Read existing program.
*/
public class ReadProgram extends Command<App>{
	/**
	* @param receiver
	*/
	public ReadProgram(App receiver){
		super(Label.READ_PROGRAM, receiver);
	}

	/**
	* @see pt.utl.ist.po.ui.Command#execute()
	*/
	@Override
	public final void execute(){
		String  filepath = "";
		try{
			filepath = entity().readString(Message.programFileName());
			ProgramReaderText prt = new ProgramReaderText();
			prt.programRead(entity().getCurrentInterpreter(), filepath);
		}
		catch(BadSourceException e1){
			entity().println(Message.fileNotFound(filepath));
		}
		catch(BadNumberException | InvalidExpressionException | NullPointerException e2){
			e2.printStackTrace();
		}
	}
}