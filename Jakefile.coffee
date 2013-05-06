require 'shelljs/global'

task 'default', ->
	echo 'okok'

task 'build', ->
	echo 'Building solution...'
	
	if process.platform is 'win32'
		msbuild = '"C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe"'
	else if process.platform is 'linux'
		msbuild = 'xbuild'

	msbuild += ' src/Frankie.sln /verbosity:minimal /t:Clean /t:Build /p:Configuration=Release'

	exec msbuild

	console.log msbuild