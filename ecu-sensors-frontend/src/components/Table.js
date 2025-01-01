const Table = ({ data }) => {
    if (!Array.isArray(data) || data.length === 0) {
      return <div className="text-center mt-4">No data available.</div>;
    }
  
    return (
      <div className="overflow-x-auto shadow-md">
        <table className="min-w-full bg-white border border-gray-200">
          <thead className="bg-gray-100">
            <tr>
              <th className="px-4 py-2 text-left">Instancja</th>
              <th className="px-4 py-2 text-left">Typ czujnika</th>
              <th className="px-4 py-2 text-left">Wartość</th>
              <th className="px-4 py-2 text-left">Data</th>
            </tr>
          </thead>
          <tbody>
            {data.map((item) => (
              <tr key={item.id} className="border-t">
                <td className="px-4 py-2">{item.instanceId}</td>
                <td className="px-4 py-2">{item.sensorType}</td>
                <td className="px-4 py-2">{item.value}</td>
                <td className="px-4 py-2">{new Date(item.timestamp).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  };
  
  export default Table;