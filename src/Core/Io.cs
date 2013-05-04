using System.IO;
using System.Text;
using System.Threading;

namespace RDumont.Frankie.Core
{
    public class Io
    {
        #region File

        /// <summary>
        /// Finds a list of files in the given path, including its subdirectories
        /// </summary>
        /// <param name="path">The path in which to find files</param>
        /// <param name="filter">The search pattern (ex.: *.txt)</param>
        public virtual string[] FindFilesRecursively(string path, string filter)
        {
            return Directory.GetFiles(path, filter, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        public virtual void DeleteFile(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Copies a file from one location to another
        /// </summary>
        /// <param name="fromPath">The path of the file to be copied</param>
        /// <param name="toPath">The path where to copy the file</param>
        /// <param name="overwrite">Whether the destination should be overwritten if it exists</param>
        public virtual void CopyFile(string fromPath, string toPath, bool overwrite)
        {
            var folder = Path.GetDirectoryName(toPath);
            if (!DirectoryExists(folder)) CreateDirectory(folder);
            File.Copy(fromPath, toPath, overwrite);
        }

        /// <summary>
        /// Writes a text file
        /// </summary>
        /// <param name="path">The path of the file to be written</param>
        /// <param name="contents">The contents of the file, in UTF8</param>
        public virtual void WriteFile(string path, string contents)
        {
            File.WriteAllText(path, contents, Encoding.UTF8);
        }

        /// <summary>
        /// Reads a text file, in optionally more than one attempt
        /// </summary>
        /// <param name="path">The path of the file to be read</param>
        /// <param name="attempts">The maximum number of attempts to open the file, if it does not succeed</param>
        /// <param name="interval">
        /// The base interval, in milliseconds, between tries.
        /// This number grows exponentially with the attempt number
        /// </param>
        public virtual string ReadFile(string path, int attempts = 1, int interval = 3)
        {
            var currentAttempt = 1;
            while (true)
            {
                try
                {
                    return File.ReadAllText(path, Encoding.UTF8);
                }
                catch (IOException)
                {
                    if (currentAttempt == attempts) throw;
                    Thread.Sleep(interval ^ currentAttempt);
                    currentAttempt++;
                }
            }
        }

        #endregion

        #region Directory

        /// <summary>
        /// Checks whether a directory exists or not
        /// </summary>
        /// <param name="path">The path of the directory to be checked</param>
        public virtual bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Creates a new directory
        /// </summary>
        /// <param name="path">The path of the directory to be created</param>
        public virtual DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        #endregion
    }
}