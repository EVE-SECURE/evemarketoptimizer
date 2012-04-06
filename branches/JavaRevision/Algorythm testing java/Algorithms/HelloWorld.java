import java.io.File;

public class HelloWorld {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		//System.out.println("Comienza main()");
		File path = new File("C:/Users/Greitone/Desktop");		
		String[] l = path.list(new DirFilter("[a-hA-H].*"));
		for(String f : l)
		{
			File fl = new File("C:/Users/Greitone/Desktop/" + f);
			System.out.println(
					"Absolute path: " + fl.getAbsolutePath() +
					"\n Can read: " + fl.canRead());
		}
		
	}
	

}

