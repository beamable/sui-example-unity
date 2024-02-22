using Beamable.Common;
using MongoDB.Driver;

namespace Beamable.Server
{
	[StorageObject("SuiStorage")]
	public class SuiStorage : MongoStorageObject
	{
	}

	public static class SuiStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for SuiStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> SuiStorageDatabase(this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<SuiStorage>();

		/// <summary>
		/// Gets a MongoDB collection from SuiStorage by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="SuiStorageCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> SuiStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<SuiStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from SuiStorage by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="SuiStorageCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> SuiStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<SuiStorage, TCollection>();
	}
}
