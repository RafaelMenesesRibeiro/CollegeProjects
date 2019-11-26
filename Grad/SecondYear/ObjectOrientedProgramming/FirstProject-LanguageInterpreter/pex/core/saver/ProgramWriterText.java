package pex.core.saver;

import java.io.IOException;
import java.io.BufferedWriter;
import java.io.FileWriter;

import pex.core.saver.ProgramWriter;
import pex.core.Interpreter;
import pex.core.Program;

public class ProgramWriterText implements ProgramWriter{
	@Override
	public Program programWrite(Interpreter interp, String progName, String filepath) throws IOException{
		Program prog = interp.programFind(progName);
		if (prog == null)
			return null;
		BufferedWriter bw = new BufferedWriter(new FileWriter(filepath));
		bw.write(prog.getAsText());
		bw.close();
		return prog;
	}
}