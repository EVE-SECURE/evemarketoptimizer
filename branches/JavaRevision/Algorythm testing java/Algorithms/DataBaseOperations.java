import java.sql.*; 

class DataBaseOperations
{
	
	private Connection conn = null;
	private String serverName="localhost";
	private String port = "1433";
	private String username = "JavaUser";
	private String password = "leochase23";
	private String url;
	private Statement stm;
	private ResultSet rs;
	
	public static void main(String[] args) throws SQLException{
		DataBaseOperations dbo = new DataBaseOperations(Database.EVEDB);
		dbo.connect();
		ResultSet rst = dbo.query("SELECT * FROM dbo.mapSolarSystemJumps");
		int count = 0;
		while(rst.next()){
			System.out.print(rst.getString("fromSolarSystemID") + " ");
			System.out.println(rst.getString("toSolarSystemID"));
			count++;
		}
		System.out.println(count);
	}
	
	public DataBaseOperations(Database database){
		this.url = "jdbc:sqlserver://"+serverName+":"+port+";DatabaseName="+database.toString()+";User="+username+";Password="+password+";";
	}
	
	public void connect (){ 
		try{ 
			Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
			this.conn = DriverManager.getConnection(url);
			if(conn!=null){
				System.out.println("Successfully connected to database: " + url);
				this.stm = conn.createStatement();
			}
		}
		catch (SQLException ex){ 
			System.err.println("Error while connecting to database: " + url); 
			System.err.println(ex.getMessage());
			ex.printStackTrace();
		}
		catch (ClassNotFoundException ex){
			System.err.println("Class not found");
			System.err.println(ex.getMessage());
			ex.printStackTrace();
		}
	}
	
	public ResultSet query(String queryText) throws SQLException{
		rs = this.stm.executeQuery(queryText);
		return rs;
	}
	
	
	
	public enum Database{
		EVEDB,EVEMarketDB;
	}
}