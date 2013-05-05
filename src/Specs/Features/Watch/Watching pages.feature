Feature: Watching pages
	In order to quickly visualize changes to a page
	As a site owner
	I want to have frankie watch it for changes

Scenario: Create a new page
	Given the default directory structure
	 And the in-memory logger
	 And that Frankie is watching my folder
	When I create the file 'about.html'
		"""
		<h1>About me</h1>
		"""
	 And wait for the watcher to finish
	Then no errors should be logged
	 And there should be an 'about.html' text file
		"""
		<h1>About me</h1>
		"""
