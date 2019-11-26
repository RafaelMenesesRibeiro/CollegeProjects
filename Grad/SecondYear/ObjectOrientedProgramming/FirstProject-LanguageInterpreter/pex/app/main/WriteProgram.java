package pex.app.main;

import java.io.IOException;
import java.lang.NullPointerException;

import pex.app.App;
import pex.core.saver.ProgramWriterText;
import pt.utl.ist.po.ui.Command;

/**
* Write (save) program to file.
*/
public class WriteProgram extends Command<App>{
	/**
	* @param receiver
	*/
	public WriteProgram(App receiver){
		super(Label.WRITE_PROGRAM, receiver);
	}

	/**
	* @see pt.utl.ist.po.ui.Command#execute()
	*/
	@Override
	public final void execute(){
		String progName = "";
		String progFilepath = "";
		try{
			App app = entity(); 
			progName = app.readString(Message.requestProgramId());
			progFilepath = app.readString(Message.programFileName());
			ProgramWriterText pwt = new ProgramWriterText();
			if (pwt.programWrite(entity().getCurrentInterpreter(), progName, progFilepath) == null)
				entity().println(Message.noSuchProgram(progName));
		}
		catch(IOException | NullPointerException e1){
			e1.printStackTrace();
		}
	}
}