using System;

public class I3DModel
{
    public string Name { get; set; }
    public I3DFile[] Files { get; set; }
    public I3DMaterial[] Materials { get; set; }
    public I3DShape[] Shapes { get; set; }
}
