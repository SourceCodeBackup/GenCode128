# GenCode128 - A Code128 Barcode Generator

http://www.codeproject.com/Articles/14409/GenCode-A-Code-Barcode-Generator

## Introduction

This is a simple library that lets you do one thing very easily: generate an Image for a Code128 barcode, with a single line of code. This image is suitable for print or display in a WinForms application, or even in ASP.NET.

## Background

Support for other barcode symbologies can be easily found because they're easy to create. Basic Code39 barcodes, for example, can be produced using nothing but a simple font, and it seems like you can't hardly swing a cat without hitting a Code39 font or imageset for download.

However, Code39 has deficiencies. The naive font-derived output doesn't afford any error checking. In their standard configuration, most scanners can only recognize 43 different symbols (configuration changes may fix this, but you may not have that luxury with your users' choice of hardware configuration). And Code39 is fairly verbose, requiring a large amount of space for a given message.

Code128, on the other hand, has out-of-the-box support for all 128 low-order ASCII characters. It has built-in error detection at the character and message level, and is extremely terse. Unfortunately, producing a reasonable encoding in this symbology is an "active" process. You must analyze the message for the optimal encoding strategy, and you must calculate a checksum for the entire message.

In my case, it was an absolute necessity to encode control characters. My application's design demanded that the user be able to trigger certain menu shortcut keys by scanning a barcode on a page. But since I've got no control over the scanners that my users are employing, the Code39EXT symbology wasn't a good candidate.

A search yielded several Code128 controls, but these had two important deficiencies. First, they were controls. That would be fine if I just wanted to produce a barcode on the page, but I wanted to use them as images in a grid, so I needed a means of obtaining a raw GDI+ Image object. Second, they were fairly expensive -- enough that a license covering all of our developers would cost more than my time to roll my own.

## Using the code

### Basic usage

As promised, producing the barcode Image is as simple as a single line of code. Of course, you'll still need code lines necessary to put that Image where it needs to go.

Here's a chunk from the sample application. In it, I respond to a button click by generating a barcode based on some input text, and putting the result into a `PictureBox` control:

```cs
private void cmdMakeBarcode_Click(object sender, System.EventArgs e)
{
   try
   {
      Image myimg = Code128Rendering.MakeBarcodeImage(txtInput.Text, 
                                         int.Parse(txtWeight.Text), true);
      pictBarcode.Image = myimg;
   }
   catch (Exception ex)
   {
      MessageBox.Show(this, ex.Message, this.Text);
   }
}
```

Obviously, the meat of this is the first line following the try. For the caller, there's just one interesting method in the whole library:

```cs
GenCode128.Code128Rendering.MakeBarcodeImage( string InputData, 
                              int BarWeight, bool AddQuietZone )
```

(That's the GenCode128 namespace, in a static class called `Code128Rendering`). Since this is a static class, you don't even need to worry about instantiating an object.

There are three parameters:

* `string InputData`
  * The message to be encoded
* `int BarWeight`
  * The baseline width of the bars in the output. Usually, 1 or 2 is good.
* `bool AddQuietZone`
  * If false, omits the required white space at the start and end of the barcode. If your layout doesn't already provide good margins around the Image, you should use true.

You can get a feel for the effect of these values by playing with the sample application. While you're at it, try printing out some samples to verify that your scanners can read the barcodes you're planning to produce.

### Printing

A barcode library is pretty much useless if you don't use it to print. You can't very well scan the screen. It's been quite a long time since I had printed anything from a Windows application, and it took a little while to remember how. If you need a quick reminder like I did, take a look at the event that the demo app's Print button calls.

### What you should be aware of

First of all, I don't have any exception handling built into the library itself. For your own safety, you should put `try/catch` blocks around any calls to the library.

The solution comprises three projects. One is the library itself, one is the demo application, and then there is the unit test code. I used NUnit by way of http://TestDriven.net. If you don't have that, then Visual Studio is going to complain. Since it's just test code, you can safely drop it and still use the library successfully.

Another point is the required vertical height of the barcode. The spec requires that the image be either 1/4" or 15% of the overall width, whichever is larger. Since I don't have any control of the scaling you're using when outputting the image, I didn't bother implementing the 1/4" minimum. This means that for very short barcode, the height might be illegally small.

Code128's high information density derives partly from intelligently shifting between several alternate codesets. Obtaining the optimal encoding is, as far as I can tell, a "hard" problem (in the sense of discrete math's non-polynomial problems like the Traveling Salesman). The difference between the best possible solution and my pretty good one should be small, and doesn't seem worth the effort.

My algorithm for obtaining a "pretty good" encoding involves a single-character look-ahead.

* If the current character can be encoded in the current codeset, then it just goes ahead and does so.
* Otherwise, if the next character would be legal in this codeset, it temporarily shifts into the alternate codeset for just this character.
* Else, both this character and the next would need a shift, so instead it changes codesets to avoid the shifts.

A similar decision has to be made about which codeset to start the encoding in. To solve this, I check the first two characters of the string, letting them "vote" to see which codeset they prefer. If there's a preference for one codeset, I choose it; otherwise, I default to codeset B. This is because codeset A allows uppercase alpha-numerics plus control characters, while codeset B allows upper and lower alpha-numerics; I assume that you're more likely to want lowercase than control characters.

Finally, there is an optimization in the Code128 spec for numeric-only output that I didn't take advantage of. Long runs of digits can be encoded in a double density codeset. Accounting for this in my already-ugly look-ahead algorithm would have taken a lot more effort -- for a feature that I don't need. But if you have lots of digits and space is tight, you might look at enhancing this.

## Points of interest

I suppose that anyone examining my source code will wonder why in the world my table of bar width has two extra columns. In any sane universe, there should be six columns rather than eight. This was a compromise to allow for the oddball STOP code, which has seven bars rather than six. I could have implemented a special case for just this code, but that was too distasteful.

Instead, I added extra zero-width columns to everything else, making the data equivalent in all cases. For every bar that comes up with a zero width, nothing is output, so nothing is harmed.

Of course, the choice between six or eight columns just begs the question: why not seven? This is to accommodate an optimization in the rendering code. By pre-initializing the entire image to white, I can avoid needing to draw the white bars. Thus, I grab bar widths in groups of two. The first one is the black one, and I draw that normally (unless its width is zero). The second one is white, but there's white already there, so I can just skip the area that would have occupied.

If anyone's keeping score, this is my second attempt at truly Test-Driven Development. On the whole, I think this worked out pretty well. Especially, at the lower levels of code, I'm pretty confident of the code. However, the highest level -- where the output is just an Image -- seemed impractical to be tested in this way.

One problem I've got with the TDD, though, is code visibility. Optimally, this library should have exactly **one** publicly-visible class, with one public method. However, my test code forces me to expose all of the lower-level stuff that the end caller should never know about. If TDD in C# has developed a good answer to that, I haven't yet stumbled upon it.
