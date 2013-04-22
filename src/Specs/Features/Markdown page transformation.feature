Feature: Markdown page transformation
	In order to save time on writing complex markup
	As a site owner
	I want to generate a page using Markdown

Background:
	Given the default directory structure
	 And the in-memory logger

Scenario: Transform a markdown page with default template
	Given the '_page' template
		"""
		This is a page.
		{{ contents }}
		"""
	 And the 'my-page.md' text file
		"""
		**My page**
		"""
	When I run Frankie
	Then there should be a 'my-page.html' text file
		"""
		This is a page.
		<p><strong>My page</strong></p>
		"""
	 And no errors should be logged
